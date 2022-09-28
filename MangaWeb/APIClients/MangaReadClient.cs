using HtmlAgilityPack;
using MangaWeb.APIClient.Services;
using MangaWeb.Models;
using RestSharp;
using System.Text.RegularExpressions;

namespace MangaWeb.APIClient
{
    public class MangaReadClient : RestClientApi
    {
        public MangaRead GetMangaReadModelByTitle(string title)
        {
            return null;
        }

        public async Task<MangaReadPage> GetPageByTitleVolumeChapter(string title)
        {
            RestClient.ChangeBaseUrlOn(await GetUrlOfMangaByTitle(title));
            var response = await RestClient.ExecuteAsync(RequestBuilder.CreateRequest().GetRequest());
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.Content);
            var mangaPagesLinks = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'media-chapter__name')]/a").Select(x=>x.GetAttributeValue("href","")).Distinct().ToArray();
            var imgesUrlByCapterNumber = new Dictionary<int, string[]>();
            for(var i = 0; i < mangaPagesLinks.Length; ++i)
            {
                RestClient.ChangeBaseUrlOn(mangaPagesLinks[i]);
                var pageResponse = await RestClient.ExecuteAsync(RequestBuilder.CreateRequest().GetRequest());
                htmlDocument.LoadHtml(pageResponse.Content);
                var imageUrl = htmlDocument.DocumentNode.SelectSingleNode("//div/img").GetAttributeValue("href","");
                var countOfPages = htmlDocument.DocumentNode.SelectNodes("//div/img").Count;

                var regex = new Regex("(\\d+)(?!.*\\d)");
                var listOfImagesUrl = new List<string>();
                for(var j = 1; j < countOfPages+1; ++j)
                {
                    listOfImagesUrl.Add(regex.Replace(imageUrl, j.ToString()));
                }
                imgesUrlByCapterNumber.Add(i + 1, listOfImagesUrl.ToArray());


            }
           
            return null;
        }
    }
}
