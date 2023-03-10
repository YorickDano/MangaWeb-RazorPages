using HtmlAgilityPack;
using MangaWeb.APIClients.Services;
using MangaWeb.Models;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web;

namespace MangaWeb.APIClients
{
    public class MangaReadLinksClient : RestClientApi
    {
        private RestClient RestClientRuManga;
        private RestClient RestClientEngManga;
        private List<string> Links;

        public MangaReadLinksClient()
        {
            RestClientRuManga = new RestClient("https://www.google.com/");
            RestClientEngManga = new RestClient("https://mangakakalot.com/search/");
        }

        public async Task<IEnumerable<string>> FindLinksForAsync(string title)
        {
            var removeRegex = new Regex("[;:,\\t\\r \\n()-]");
            Links = new List<string>();
            var engMangaResponseTask = RestClientEngManga.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource($"story/{removeRegex.Replace(title, "_")}").GetRequest());
            var ruMangaResponseTask = RestClientRuManga.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource($"search").AddRequestParameter("q", title + " mangalib").GetRequest());
            await Task.WhenAll(engMangaResponseTask, ruMangaResponseTask);

            var htmlEngManga = new HtmlDocument();
            var htmlRuManga = new HtmlDocument();

            htmlEngManga.LoadHtml(engMangaResponseTask.Result.Content);
            htmlRuManga.LoadHtml(ruMangaResponseTask.Result.Content);
            var engMangaElement = htmlEngManga.DocumentNode.SelectSingleNode("//div[@class='story_item']/a[@rel='nofollow']");

            Links.Add(engMangaElement?.GetAttributeValue("href", "There are no link for this manga"));

            var ruMangaElement = htmlRuManga.DocumentNode.SelectSingleNode("//a[contains(@href,'mangalib.me')]");
            if (ruMangaElement != null)
            {
                var ruMangaLink = ruMangaElement
                    .GetAttributeValue("href", "There are no link for this manga").Replace("/url?q=", "");
                ruMangaLink = HttpUtility.UrlDecode(ruMangaLink);
                var index = ruMangaLink.IndexOf("&");
                if (index >= 0)
                {
                    ruMangaLink = ruMangaLink.Substring(0, index);
                }
                Links.Add(ruMangaLink);
            }


            return Links;
        }

        public IEnumerable<string> GetLinks() => Links;

        ~MangaReadLinksClient()
        {
            RestClientRuManga.Dispose();
            RestClientEngManga.Dispose();
        }

    }
}
