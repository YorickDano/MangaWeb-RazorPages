using HtmlAgilityPack;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using RestSharp;
using System.Globalization;

namespace MangaWeb.APIClients
{
    public class ResearchMangaClient : RestClientApi
    {

        private protected const string BaseUrl = "https://myanimelist.net";

        private Manga Manga;

        public async Task<Manga> GetFullManga(string title)
        {
            Manga = Manga.CreateNew();
            var htmlDocument = new HtmlDocument();
            RestClient = new RestClient(MyAnimeListUrl);
            var mangaUrlName = await GetManagaUrl(title);
            var mangaSiteResponse = await RestClient.ExecuteAsync(
              RequestBuilder.CreateRequest().SetRequestResource(mangaUrlName).GetRequest());
            htmlDocument.LoadHtml(mangaSiteResponse.Content);
            Manga = SetTitles(htmlDocument, Manga);
            Manga = SetMangaImage(htmlDocument, Manga);
            Manga = SetDescription(htmlDocument, Manga);
            Manga = SetMangaInfo(htmlDocument, Manga);

            return await SetMangaCharacters(htmlDocument, Manga, mangaUrlName);
        }

        public async Task<Manga> UpdateMangaAsync(Manga currentManga)
        {
            return await GetFullManga(currentManga.OriginTitle);
        }

        protected async Task<string> GetManagaUrl(string title)
        {
            var htmlDocument = await GetHtmlOfMangaSearchFromMyAnimeListAsync(title);
            var mangaLink = htmlDocument.DocumentNode.SelectSingleNode("//td[contains(text(),'Manga')]/parent::tr//a")
                .GetAttributeValue("href", DefaultValuesManager.DefaultMangaLink);

            return mangaLink.Replace(BaseUrl, "");
        }

        protected async Task<HtmlDocument> GetHtmlOfMangaSearchFromMyAnimeListAsync(string title)
        {
            var htmlDocument = new HtmlDocument();
            var request = RequestBuilder.CreateRequest()
              .SetRequestResource("/manga.php")
              .AddRequestParameter("q", title)
              .AddRequestParameter("cat", "manga")
              .GetRequest();
            RestClient = new RestClient(BaseUrl);
            var mangaSearchSiteResponse = RestClient.Execute(request);
            htmlDocument.LoadHtml(mangaSearchSiteResponse.Content);
            Console.WriteLine(RestClient.BuildUri(request));
            return htmlDocument;
        }

        private Manga SetTitles(HtmlDocument htmlDocument, Manga manga)
        {
            var titlesElement = htmlDocument.DocumentNode.SelectSingleNode("//h1//span[@itemprop='name']");
            manga.OriginTitle = titlesElement.GetDirectInnerText();
            return manga;
        }

        private Manga SetMangaImage(HtmlDocument htmlDocument, Manga manga)
        {
            manga.MangaImageUrl = htmlDocument.DocumentNode
                .SelectSingleNode("//img[@itemprop='image']")
                .GetAttributeValue("data-src", DefaultValuesManager.DefaultMangaImageUrl);

            return manga;
        }

        private Manga SetDescription(HtmlDocument htmlDocument, Manga manga)
        {
            var fullDescription = htmlDocument.DocumentNode
                .SelectSingleNode("//span[@itemprop='description']").GetDirectInnerText().Replace("&#039;s", "");
            manga.Description = fullDescription.Remove(fullDescription.LastIndexOf('.') + 1);
            return manga;
        }
        private Manga SetMangaInfo(HtmlDocument htmlDocument, Manga manga)
        {
            var documentNode = htmlDocument.DocumentNode;
            manga.CountOfVolume = int.TryParse
                (documentNode.SelectSingleNode("//span[contains(text(),'Volume')]/..")
                .GetDirectInnerText().Trim(), out DefaultNumber) ? DefaultNumber : -1;
            manga.CountOfChapters = int.TryParse
                (documentNode.SelectSingleNode("//span[contains(text(),'Chapters')]/..")
                .GetDirectInnerText().Trim(), out DefaultNumber) ? DefaultNumber : -1;
            manga.Status = (MangaStatus)Enum.Parse(typeof(MangaStatus), documentNode.SelectSingleNode("//span[contains(text(),'Status')]/..").GetDirectInnerText().Trim());
            manga.Published = documentNode.SelectSingleNode("//span[contains(text(),'Published')]/..").GetDirectInnerText().Trim();
            var yearStr = manga.Published.Split(' ')[3];
            var yearTry = 0;
            manga.YearOfIssue = int.TryParse(yearStr, out yearTry) ? yearTry : 0;
            manga.Genres = new List<string>(documentNode.SelectNodes("//span[@itemprop='genre']").Select(x => x.GetDirectInnerText()));
            manga.Autors = new List<string>(documentNode.SelectNodes("//span[contains(text(),'Authors')]/../a").Select(x => x.GetDirectInnerText().Replace(",", string.Empty)));
            manga.Popularity = int.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Popularity')]/..").GetDirectInnerText().Trim().TrimStart('#'));
            var rankedTry = 0;
            manga.Ranked = int.TryParse(documentNode.SelectSingleNode("//span[contains(text(),'Ranked')]/..").GetDirectInnerText().Trim().TrimStart('#'), out rankedTry) ? rankedTry : -1;
            manga.Score = double.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Score')]/../span/span").GetDirectInnerText(), new NumberFormatInfo { NumberDecimalSeparator = "." });

            return manga;
        }

        private int DefaultNumber = -1;

        private async Task<Manga> SetMangaCharacters(HtmlDocument htmlDocument, Manga manga, string mangaUrlName)
        {
            var mangaCharacterClient = new MangaCharacterClient();
            return await mangaCharacterClient.GetAllCharacters(manga.OriginTitle, manga, mangaUrlName); ;
        }
    }
}
