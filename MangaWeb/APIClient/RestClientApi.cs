using HtmlAgilityPack;
using MangaWeb.APIClient.Services;
using RestSharp;
using System.Net;
using System.Text.RegularExpressions;

namespace MangaWeb.APIClient
{
    public class RestClientApi
    {
        private RestClient RestClient;
        private readonly Uri Url =  new Uri("https://www.google.by/");
        RequestBuilder requestBuilder = new RequestBuilder();

        public RestClientApi()
        {
            RestClient = new RestClient(Url);    
        }

        public async Task<string> GetUrlOfMangaByTitle(string title)
        {
            var respounce = await SearchFor(title + " манга");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(respounce);
            var f = htmlDocument.DocumentNode.SelectNodes("//a[contains(@href,'https://mangalib.me/')]");
            var linkTag = f.FirstOrDefault();
            var link = linkTag.GetAttributeValue("href", "https://mangalib.me/");
            link = link.Remove(0, 7);
            link = link.Remove(link.IndexOf("&amp"));
            return link;
        }

        public async Task<string> GetImageUrlByTitle(string title)
        {
            requestBuilder.CreateRequest()
                .SetRequestResource("search")
                .AddRequestParameter("q", title + " манга")
                .AddRequestParameter("tbm", "isch");
            var respounce = await RestClient.ExecuteAsync(requestBuilder.GetRequest());
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(respounce.Content);
            var linkTag = htmlDocument.DocumentNode.SelectNodes("//img[contains(@src,'https://encrypted')]").FirstOrDefault();

            return linkTag.GetAttributeValue("src", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSb1kKGBpZv8tOMiEi2yTJzr9sPAlRbnQe2gY_F_C7qfn0jInMVbFFjuNesYkk&amp;s");
        }

        private async Task<string> GetHtmlOf(string url)
        {
            using var client = new WebClient();
            return client.DownloadString(url);
        }

        private async Task<string> SearchFor(string title)
        {
            requestBuilder.CreateRequest()
               .SetRequestResource("search")
               .AddRequestParameter("q", title);
            var respounce = await RestClient.ExecuteAsync(requestBuilder.GetRequest());

            return respounce.Content;
        } 
    }
}
