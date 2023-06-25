using HtmlAgilityPack;
using MangaWeb.APIClients.Models;
using MangaWeb.APIClients.Services;
using MangaWeb.Models;
using Newtonsoft.Json;
using RestSharp;
using RuMangaUrlInfo = MangaWeb.APIClients.Models.RuMangaUrlInfoModel;

namespace MangaWeb.APIClients
{
    public class ResearchRuMangaClient
    {
        private RestClient Client { get; set; }
        private readonly string BaseUrl = "https://shikimori.me";

        private RequestBuilder RequestBuilder { get; set; }

        public ResearchRuMangaClient()
        {
            RequestBuilder = new RequestBuilder();
            Client = new RestClient(BaseUrl);

        }

        public async Task<Manga> GetManga(string title, IEnumerable<string> mangaTitelsExists)
        {
            var manga = new Manga();
            var mangaMainInfo = await GetMangaMainInfo(title);
            if (mangaMainInfo == null || mangaTitelsExists.Contains(mangaMainInfo.russian) )
            {
                return null;
            }
            var htmlDocument = new HtmlDocument();
            manga = await SetMangaInfo(manga, mangaMainInfo, mangaMainInfo.url);
            var mangaCharctersPageResponse = await Client.ExecuteAsync(
                RequestBuilder.CreateRequest().SetRequestResource(mangaMainInfo.url + "/characters")
                .AddHeadersForShikimori().GetRequest());
            htmlDocument.LoadHtml(mangaCharctersPageResponse.Content);
            var mainCharactersBlock = htmlDocument.DocumentNode
                .SelectSingleNode("//div[contains(@class,'cc-characters')]");
            var mainCharactersNames = mainCharactersBlock?.InnerText;
            if (mainCharactersNames is not null)
            {
                manga = await SetMangaCharacters(manga, htmlDocument, mainCharactersNames);
            }
            var mangaLinks = await new MangaReadLinksClient().FindLinksForAsync(title);
            manga.ReadLinks = mangaLinks;

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
                htmlCharactersDocuments.Add(await requestExecutor.SendRequestAsync(request));
            }

            var characters = await GetCharacters(htmlCharactersDocuments, mainCharactersNames);

            manga.Characters = new List<MangaCharacter>(characters);

            return manga;
        }

        public async Task<IEnumerable<MangaCharacter>> GetCharacters(IEnumerable<HtmlDocument> documents, string mainCharactersNames)
        {
            var characters = new List<MangaCharacter>();
            foreach (var document in documents)
            {
                string characterName = document.DocumentNode.SelectSingleNode("//div[@class='value']")?.InnerText;
                characters.Add(new MangaCharacter()
                {
                    Name = characterName,
                    ImageUrl = document.DocumentNode.SelectSingleNode("//div[@class='c-poster']//img")?.Attributes["src"].Value,
                    Description = document.DocumentNode.SelectSingleNode("//div[@class='text' and @itemprop='description']")?.InnerText,
                    IsMain = mainCharactersNames.Contains(characterName)
                });
            }

            return characters;
        }

        public async Task<Manga> SetMangaInfo(Manga manga, RuMangaUrlInfo.Root mangaMainInfo, string url)
        {
            manga.Language = Language.ru;
            manga.OriginTitle = mangaMainInfo.russian;
            manga.MangaImageUrl = BaseUrl + mangaMainInfo.image.original;
            manga.Type = char.ToUpper(mangaMainInfo.kind[0]) + mangaMainInfo.kind.Substring(1).Replace('_', ' ');
            try
            {
                manga.Score = Convert.ToSingle(mangaMainInfo.score);
            }
            catch (Exception ex) { manga.Score = Convert.ToSingle(mangaMainInfo.score.Replace(".", ",")); }
            manga.CountOfVolume = mangaMainInfo.volumes == 0 && mangaMainInfo.kind != "manga" ? 1 : mangaMainInfo.volumes;
            manga.CountOfChapters = mangaMainInfo.chapters == 0 ? -1 : mangaMainInfo.chapters;
            manga.Published = mangaMainInfo.aired_on + " - " + mangaMainInfo.released_on;
            manga.YearOfIssue = int.Parse(mangaMainInfo.aired_on.Remove(4));
            manga.Status = mangaMainInfo.status == "released" ? MangaStatus.Издано : MangaStatus.Выходит;

            var restMangaInfo = JsonConvert.DeserializeObject<RuMangaInfoModel.Root>((await Client.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource($"/api/mangas/{mangaMainInfo.id}").GetRequest())).Content);
            manga.Genres = restMangaInfo.genres.Select(x => x.russian);
            manga.Description = restMangaInfo.description;

            var htmlDocument = new HtmlDocument();
            var autorsPageResponse = await Client.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource(url + "/resources").GetRequest());
            if (autorsPageResponse.Content!= "age_restricted")
            {
                htmlDocument.LoadHtml(autorsPageResponse.Content);

                manga.Authors = htmlDocument.DocumentNode
                   .SelectNodes("//div[contains(@class,'authors')]//div[contains(@class,'authors')]//div[@class='name']/a")
                   .Select(x => x.InnerText);
            }
            else
            {
                manga.Authors = new List<string>(); 
            }

            return manga;
        }

        private async Task<RuMangaUrlInfo.Root> GetMangaMainInfo(string title)
        {
            var response = await Client.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource("/api/mangas").AddHeadersForShikimori()
                .AddRequestParameter("search", title).GetRequest());
            try
            {
                var responseObj = JsonConvert.DeserializeObject<List<RuMangaUrlInfo.Root>>(response.Content)[0];

                return responseObj;
            }
            catch (Exception ex) { return null; }
        }

        public async Task<Manga> UpdateMangaAsync(Manga currentManga, IEnumerable<string> mangaTitelsExists)
        {
            return await GetManga(currentManga.OriginTitle, mangaTitelsExists);
        }
    }
}
