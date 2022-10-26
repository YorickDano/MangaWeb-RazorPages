using HtmlAgilityPack;
using MangaWeb.APIClient;
using MangaWeb.APIClient.Services;
using MangaWeb.Managers;
using MangaWeb.Models;
using NuGet.Packaging;
using RestSharp;

namespace MangaWeb.APIClients
{
    public class MangaCharacterClient : ReseachMangaClient
    {
        public async Task<FullManga> GetAllCharacters(string title, FullManga fullManga)
        {
            var htmlDocument = new HtmlDocument();
            var charactersLink = await GetManagaUrl(title);
            var requestCharactersList = RequestBuilder.CreateRequest().SetRequestResource($"{charactersLink}/characters").GetRequest();
            var responseCharactersList = await RestClient.ExecuteAsync(requestCharactersList);

            htmlDocument.LoadHtml(responseCharactersList.Content);
            var charactersLinks = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'picSurround')]/a")
                .Select(x => x.Attributes["href"].Value);
            Console.WriteLine(String.Join(" ", charactersLinks));
            var responsesCharactersPages = new List<RestResponse>();

            foreach(var characterLink in charactersLinks.Select(x=>x.Replace(BaseUrl,"")))
            {
                var requestCharacter = RequestBuilder.CreateRequest().SetRequestResource(characterLink).GetRequest();
                Console.WriteLine(RestClient.BuildUri(requestCharacter));
                htmlDocument.LoadHtml((await RestClient.ExecuteAsync(requestCharacter)).Content);
                // name ismain mainImage Images desc
                fullManga.Characters.Add(GetMangaCharacter(htmlDocument));

            }
            var imagesUrlsForCharacters = await GetImagesUrlsForCharacters(charactersLinks.Select(x => x + "/pics"));
            for(var i = 0; i < fullManga.Characters.Count; ++i)
            {
                fullManga.Characters[i].ImagesUrls = imagesUrlsForCharacters[i];
            }

            return fullManga;
        }
        private MangaCharacter GetMangaCharacter(HtmlDocument htmlDocument)
        {
            var name = htmlDocument.DocumentNode.SelectSingleNode("//h2[contains(@class,'normal_header')]").InnerText;
            var imageUrl = htmlDocument.DocumentNode.SelectSingleNode("//div[not(@class)]/a/img").Attributes["data-src"].Value;
            var isMain = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(text(),'Mangaography')]/following::small[1]").InnerText == "Main" ? true : false;
            var description = htmlDocument.DocumentNode.SelectSingleNode("//td[@valign='top' and not(@class)]").GetDirectInnerText().Replace("\n","").Replace("\t","").Replace("&#039;s", "").Replace("&nbsp;","").Replace("&gt;", "");

            return new MangaCharacter(name, imageUrl, isMain, description);
        }

        private async Task<List<List<string>>> GetImagesUrlsForCharacters(IEnumerable<string> characterImagesLinks)
        {
            var allImages = new List<List<string>>();
            
            foreach (var characterImagesLink in characterImagesLinks.Select(x=>x.Replace(BaseUrl,"")))
            {
                var imagesForCharacter = new List<string>();
                var requestImagesOfCharacter = RequestBuilder.CreateRequest()
                    .SetRequestResource(characterImagesLink).GetRequest();
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml((await RestClient.ExecuteAsync(requestImagesOfCharacter)).Content);
                imagesForCharacter.AddRange(htmlDocument.DocumentNode.SelectNodes("//img[contains(@class,'portrait')]").Select(x => x.Attributes["data-src"].Value));
                imagesForCharacter.RemoveAt(0);
                allImages.Add(imagesForCharacter);
            }

            return allImages;
        }
    }
}
