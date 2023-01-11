using HtmlAgilityPack;
using MangaWeb.APIClients.Services;
using MangaWeb.Managers;
using MangaWeb.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Text.Json;
using System.Text.Json.Nodes;
using static MangaWeb.Models.ShikimoriMangaSearchJson;

namespace MangaWeb.APIClients
{
    public class ResearchRuMangaClient
    {
        private RestClient Client { get; set; }
        private readonly string BaseUrl = "https://shikimori.one";

        private RequestBuilder RequestBuilder { get; set; }

        public ResearchRuMangaClient()
        {
            RequestBuilder = new RequestBuilder();
            Client = new RestClient(BaseUrl);
            
        }

        public async Task<Manga> GetManga(string title)
        {
            var manga = new Manga();
            var url = await GetMangaUrl(title);
            var mangaPageResponse = await Client.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource(url.Remove(0, 1)).AddHeadersForShikimori().GetRequest());
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(mangaPageResponse.Content);
            manga = await SetMangaInfo(manga, htmlDocument, url);
            var mangaCharctersPageResponse = await Client.ExecuteAsync(
                RequestBuilder.CreateRequest().SetRequestResource(url + "/characters")
                .AddHeadersForShikimori().GetRequest());
            htmlDocument.LoadHtml(mangaCharctersPageResponse.Content);
            var mainCharactersBlock = htmlDocument.DocumentNode
                .SelectSingleNode("//div[contains(@class,'cc-characters')]");
            var mainCharactersNames = mainCharactersBlock.InnerText;
                
            manga = await SetMangaCharacters(manga, htmlDocument, mainCharactersNames);
            Client.Dispose();
            return manga;
        }

        public async Task<Manga> SetMangaCharacters(Manga manga, HtmlDocument htmlDocument, string mainCharactersNames)
        {
            var characterUrls = htmlDocument
                .DocumentNode.SelectNodes("//a[contains(@class,'cover anime-tooltip')]")
                .Select(x => x.Attributes["href"].Value.Replace(BaseUrl, string.Empty));

            var requests = characterUrls.Select(x => RequestBuilder.CreateRequest()
            .SetRequestResource(x).AddHeadersForShikimori().GetRequest());
            var requestExecutor = new RequestExecutor(BaseUrl);
            var htmlCharactersDocuments = new List<HtmlDocument>();
            foreach (var request in requests)
            {
                Thread.Sleep(100);
                htmlCharactersDocuments.Add(await requestExecutor.SendRequestAsync(request));
            }

            var characters = await GetCharacters(htmlCharactersDocuments, mainCharactersNames);

            manga.Characters = new List<MangaCharacter>(characters);

            return manga;
        }

        public async Task<IEnumerable<MangaCharacter>> GetCharacters(IEnumerable<HtmlDocument> documents, string mainCharactersNames)
        {
            var characters = new List<MangaCharacter>();
            var characterName = string.Empty;
            foreach (var document in documents)
            {
                characterName = document.DocumentNode.SelectSingleNode("//div[@class='value']").InnerText;
                characters.Add(new MangaCharacter()
                {
                    Name = characterName,
                    ImageUrl = document.DocumentNode.SelectSingleNode("//div[@class='c-poster']/img").Attributes["src"].Value,
                    Description = document.DocumentNode.SelectSingleNode("//div[@class='text' and @itemprop='description']").InnerText,
                    IsMain = mainCharactersNames.Contains(characterName)
                 });
            }
      
            return characters;
        }

        public async Task<Manga> SetMangaInfo(Manga manga, HtmlDocument document, string url)
        {
            manga.Language = Language.ru;
            manga.Type = document.DocumentNode.SelectSingleNode("//div[@class='value']").InnerText;
            manga.OriginTitle = document.DocumentNode.SelectSingleNode("//h1").InnerText.Split('/')[0];
            manga.Status = document.DocumentNode.SelectSingleNode("//span[contains(@class,'anime_status')]")
                .GetAttributeValue("data-text", MangaStatus.Выходит);

            var chaptersCount = 0;
            var chaptersString = document.DocumentNode.SelectSingleNode("//div[text()='Главы:']/following-sibling::div")?.InnerText;
            manga.CountOfChapters = int.TryParse(chaptersString, out chaptersCount) ? chaptersCount : -1;
            var volumeCount = 0;
            var volumeString = document.DocumentNode.SelectSingleNode("//div[text()='Тома:']/following-sibling::div")?.InnerText;
            manga.CountOfVolume = int.TryParse(volumeString, out volumeCount) ? volumeCount : -1;
            manga.Genres = new List<string>( document.DocumentNode.SelectNodes("//a/span[@class='genre-ru']").Select(x => x.GetDirectInnerText()));
            manga.Published = document.DocumentNode.SelectSingleNode("//span[contains(@class,'b-tooltipped dotted mobile')]")
                .GetAttributeValue("title", "Не известно");
            manga.Description = document.DocumentNode.SelectSingleNode("//div[@class='b-text_with_paragraphs']").InnerText;
            manga.MangaImageUrl = document.DocumentNode.SelectSingleNode("//div[@class='c-poster']/img").GetAttributeValue("src", DefaultValuesManager.DefaultMangaImageUrl);
            manga.YearOfIssue = int.Parse(manga.Published.Split(' ')[^2]);
            manga.Score = float.Parse(document.DocumentNode.SelectSingleNode("//div[contains(@class,'score-value')]").InnerText.Replace('.',','));
            var htmlDocument = new HtmlDocument();
            var autorsPageResponse = await Client.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource(url + "/resources").GetRequest());
            htmlDocument.LoadHtml(autorsPageResponse.Content);

            manga.Autors = htmlDocument.DocumentNode
               .SelectNodes("//div[contains(@class,'c-authors')]//a/span[@class='name-ru']")
               .Select(x => x.InnerText);

            return manga;
        }

        private async Task<string> GetMangaUrl(string title)
        {
            var response = await Client.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource("/api/mangas").AddRequestParameter("search", title).GetRequest());

            var responseObj = JsonConvert.DeserializeObject<List<Root>>(response.Content)[0];

            return responseObj.url;
        }

        public async Task<Manga> UpdateMangaAsync(Manga currentManga)
        {
            return await GetManga(currentManga.OriginTitle);
        }
    }
}
