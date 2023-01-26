using HtmlAgilityPack;
using MangaWeb.APIClients.Services;
using MangaWeb.Enums;
using MangaWeb.Managers;
using RestSharp;
using System.Net;

namespace MangaWeb.APIClients
{
    public class RestClientApi
    {
        private protected readonly Uri MyAnimeListUrl = new Uri("https://myanimelist.net");
        private protected readonly Uri BaseUrl = new Uri("https://www.google.com/");
        private readonly object _locker = new();
        protected RestClient RestClient;
        protected RequestBuilder RequestBuilder = new();
        protected WebClient WebClient;


        public RestClientApi()
        {
            RestClient = new RestClient(MyAnimeListUrl);
            WebClient = new WebClient();
        }

        public async Task<string> GetUrlOfMangaByTitle(string title)
        {
            RestClient = new RestClient(BaseUrl);
            var response = await SearchFor(title + " mangalib");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response);
            var f = htmlDocument.DocumentNode.SelectNodes("//a[contains(@href,'https://mangalib.me/')]");
            var linkTag = f.FirstOrDefault();
            var link = linkTag.GetAttributeValue("href", "https://mangalib.me/");
            link = link.Remove(0, 7);
            link = link.Remove(link.IndexOf("&amp"));
            link = link.Remove(link.IndexOf("%"));
            return link;
        }

        public async Task<string> GetMangaProfieImageUrlByTitle(string title, SearchType searchType = SearchType.DefaultImage)
        {
            RestClient = new RestClient(BaseUrl);
            var responseContent = await GetImageSearchRespounseContent(title + " mangalib", searchType);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseContent);
            var linkTag = htmlDocument.DocumentNode.SelectNodes("//img[contains(@src,'https://encrypted')]").FirstOrDefault();

            return linkTag.GetAttributeValue("src", DefaultValuesManager.DefaultProfileImageUlr);
        }

        protected async Task<string> GetImageSearchRespounseContent(string title, SearchType searchType = SearchType.DefaultImage)
        {
            RestClient = new RestClient(BaseUrl);
            var searchString = searchType == SearchType.MangaImage ? title + " manga" : title;
            RequestBuilder.CreateRequest()
                .SetRequestResource("search")
                .AddRequestParameter("q", searchString)
                .AddRequestParameter("tbm", "isch");
            var response = await RestClient.ExecuteAsync(RequestBuilder.GetRequest());
            return response.Content;
        }

        protected async Task<string> GetHtmlOf(string url)
        {
            using var client = new WebClient();
            return client.DownloadString(url);
        }

        protected async Task<string> SearchFor(string title)
        {
            RestClient = new RestClient(BaseUrl);
            RequestBuilder.CreateRequest()
               .SetRequestResource("search")
               .AddRequestParameter("q", title);
            var response = await RestClient.ExecuteAsync(RequestBuilder.GetRequest());

            return response.Content;
        }

    }
}
