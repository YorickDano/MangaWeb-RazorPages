using HtmlAgilityPack;
using MangaWeb.APIClient.Services;
using MangaWeb.APIClients;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.CodeAnalysis;
using RestSharp;
using System.Globalization;

namespace MangaWeb.APIClient
{
    public class ReseachMangaClient : RestClientApi
    {

        private protected const string BaseUrl = "https://myanimelist.net";

        private FullManga FullManga { get; set; } = FullManga.Empty; 

        public async Task<FullManga> GetFullManga(string title)
        {
           var htmlDocument = new HtmlDocument();
            RestClient = new RestClient(MyAnimeListUrl);
            var mangaSiteResponse = await RestClient.ExecuteAsync(
              RequestBuilder.CreateRequest().SetRequestResource(await GetManagaUrl(title)).GetRequest());
            htmlDocument.LoadHtml(mangaSiteResponse.Content);
            FullManga = SetTitles(htmlDocument, FullManga);
            FullManga = SetMangaImage(htmlDocument, FullManga);
            FullManga = SetDescription(htmlDocument, FullManga);
            FullManga = SetMangaInfo(htmlDocument, FullManga);

            return await SetMangaCharacters(htmlDocument, FullManga);
        }

        protected async Task<string> GetManagaUrl(string title)
        {
            var htmlDocument = await GetHtmlOfMangaSearchFromMyAnimeListAsync(title);
            var mangaLink = htmlDocument.DocumentNode
                .SelectNodes("//a[contains(@href,'https://myanimelist.net/manga/')and@class='hoverinfo_trigger']")
                .FirstOrDefault().GetAttributeValue("href", DefaultValuesManager.DefaultMangaLink);

            return mangaLink.Replace(BaseUrl,"");
        }

        protected async Task<HtmlDocument> GetHtmlOfMangaSearchFromMyAnimeListAsync(string title)
        {
            var htmlDocument = new HtmlDocument();
            var request = RequestBuilder.CreateRequest()
              .SetRequestResource("/manga.php")
              .AddRequestParameter("q", title)
              .AddRequestParameter("cat", "manga")
              .GetRequest();
            var mangaSearchSiteResponse = await RestClient.ExecuteAsync(request);
            htmlDocument.LoadHtml(mangaSearchSiteResponse.Content);
            Console.WriteLine(RestClient.BuildUri(request));
            return htmlDocument;
        }

        private FullManga SetTitles(HtmlDocument htmlDocument, FullManga fullManga)
        {
            var titlesElement = htmlDocument.DocumentNode.SelectSingleNode("//h1//span[@itemprop='name']");
            fullManga.OriginTitle = titlesElement.GetDirectInnerText();
            return fullManga;
        }

        private FullManga SetMangaImage(HtmlDocument htmlDocument, FullManga fullManga)
        {
            fullManga.MangaImageUrl = htmlDocument.DocumentNode
                .SelectSingleNode("//img[@itemprop='image']")
                .GetAttributeValue("data-src", DefaultValuesManager.DefaultMangaImageUrl);

            return fullManga;
        }

        private FullManga SetDescription(HtmlDocument htmlDocument, FullManga fullManga)
        {
            var fullDescription = htmlDocument.DocumentNode
                .SelectSingleNode("//span[@itemprop='description']").GetDirectInnerText().Replace("&#039;s", "");
            fullManga.Description = fullDescription.Remove(fullDescription.LastIndexOf('.') + 1);
            return fullManga;
        }
        private FullManga SetMangaInfo(HtmlDocument htmlDocument, FullManga fullManga)
        {
            var documentNode = htmlDocument.DocumentNode;
            fullManga.CountOfVolume = int.TryParse
                (documentNode.SelectSingleNode("//span[contains(text(),'Volume')]/..")
                .GetDirectInnerText().Trim(), out DefaultNumber) ? DefaultNumber : -1;
            fullManga.CountOfChapters = int.TryParse
                (documentNode.SelectSingleNode("//span[contains(text(),'Chapters')]/..")
                .GetDirectInnerText().Trim(), out DefaultNumber) ? DefaultNumber : -1;
            fullManga.Status = (MangaStatus)Enum.Parse(typeof(MangaStatus), documentNode.SelectSingleNode("//span[contains(text(),'Status')]/..").GetDirectInnerText().Trim());
            fullManga.Published = documentNode.SelectSingleNode("//span[contains(text(),'Published')]/..").GetDirectInnerText().Trim();
            var yearStr = fullManga.Published.Split(' ')[3];
            var yearInt = 0;
            fullManga.YearOfIssue = int.TryParse(yearStr, out yearInt) ? yearInt : 0;
            fullManga.Geners = new List<string>(documentNode.SelectNodes("//span[@itemprop='genre']").Select(x => x.GetDirectInnerText()));
            fullManga.Autors = new List<string>(documentNode.SelectNodes("//span[contains(text(),'Authors')]/../a").Select(x => x.GetDirectInnerText().Replace(",", String.Empty)));
            fullManga.Popularity = int.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Popularity')]/..").GetDirectInnerText().Trim().TrimStart('#'));
            fullManga.Ranked = int.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Ranked')]/..").GetDirectInnerText().Trim().TrimStart('#'));
            fullManga.Score = double.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Score')]/../span/span").GetDirectInnerText(), new NumberFormatInfo { NumberDecimalSeparator = "." });

            return fullManga;
        }

        private int DefaultNumber = -1;

        private async Task<FullManga> SetMangaCharacters(HtmlDocument htmlDocument, FullManga fullManga)
        {
            var mangaCharacterClient = new MangaCharacterClient();
            return await mangaCharacterClient.GetAllCharacters(fullManga.OriginTitle, fullManga); ;
        }
    }
}
