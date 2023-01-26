using HtmlAgilityPack;
using MangaWeb.APIClients.Services;
using MangaWeb.Models;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
            Links = new List<string>();
            var engMangaResponseTask = RestClientEngManga.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource($"story/{title.Replace(" ","_")}").GetRequest());
            var ruMangaResponseTask = RestClientRuManga.ExecuteAsync(RequestBuilder.CreateRequest()
                .SetRequestResource($"search").AddRequestParameter("q", title + " mangalib").GetRequest());
            Task.WaitAll(engMangaResponseTask, ruMangaResponseTask);

            var htmlEngManga = new HtmlDocument();
            var htmlRuManga = new HtmlDocument();

            htmlEngManga.LoadHtml(engMangaResponseTask.Result.Content);
            htmlRuManga.LoadHtml(ruMangaResponseTask.Result.Content);
            var engMangaElement = htmlEngManga.DocumentNode.SelectSingleNode("//div[@class='story_item']/a[@rel='nofollow']");
            if(engMangaElement != null)
            {
                Links.Add(engMangaElement
               .GetAttributeValue("href", "There are no link for this manga"));
            }
            var ruMangaElement = htmlRuManga.DocumentNode.SelectSingleNode("//a[contains(@href,'mangalib.me')]");
            if(ruMangaElement != null)
            {
                var ruMangaLink = ruMangaElement
              .GetAttributeValue("href", "There are no link for this manga").Replace("/url?q=", "");

                Links.Add(ruMangaLink.Remove(ruMangaLink.IndexOf("&amp;")));
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
