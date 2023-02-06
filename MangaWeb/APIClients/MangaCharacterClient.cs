using HtmlAgilityPack;
using MangaWeb.APIClients.Services;
using MangaWeb.Managers;
using MangaWeb.Models;
using NuGet.Packaging;
using RestSharp;
using System.Linq;
using System.Net;
using System.Web;

namespace MangaWeb.APIClients
{
    public class MangaCharacterClient 
    {
        private const string MyAnimeListUrl = "https://myanimelist.net";
        private RestClient RestClient;
        private RequestBuilder RequestBuilder;
        private RequestExecutor RequestExecutor;

        public MangaCharacterClient() 
        {
            RestClient = new RestClient(MyAnimeListUrl);
            RequestBuilder = new RequestBuilder();
            RequestExecutor = new RequestExecutor(MyAnimeListUrl);
        }

        public async Task<Manga> GetAllCharacters(string title, Manga manga, int id)
        {
            var charactersLinks = await GetCharacterLinks(id, title);
            var charactersDetails = await GetCharacterDetails(charactersLinks);
            var imagesUrlsForCharacters = await GetImagesUrlsForCharacters(charactersLinks.Select(x => x + "/pics"));
            manga.Characters.AddRange(charactersDetails);
            
            for (var i = 0; i < manga.Characters.Count; ++i)
            {
                imagesUrlsForCharacters[i].Remove(manga.Characters[i].ImageUrl);
                manga.Characters[i].ImagesUrls = imagesUrlsForCharacters[i];
            }

            RestClient.Dispose();

            return manga;
        }

        private async Task<List<string>> GetCharacterLinks(int id, string title)
        {
            var htmlDocument = new HtmlDocument();
            var requestCharactersList = RequestBuilder.CreateRequest().SetRequestResource($"manga/{id}/{title}/characters").GetRequest();
            var responseCharactersList = await RestClient.ExecuteAsync(requestCharactersList);

            htmlDocument.LoadHtml(responseCharactersList.Content);

            var charactersNodes = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'picSurround')]/a");
        
            return charactersNodes == null ? new List<string>() : charactersNodes.Select(x => x.Attributes["href"].Value).ToList();
        }

        private async Task<IEnumerable<MangaCharacter>> GetCharacterDetails(List<string> characterLinks)
        {
            var requests = characterLinks.Select(x => RequestBuilder.CreateRequest()
                .SetRequestResource(x.Replace(MyAnimeListUrl, String.Empty)).GetRequest());

            var htmlCharactersDocuments = await Task.WhenAll(requests.Select(x => RequestExecutor.SendRequestAsync(x)));
            return await Task.WhenAll(htmlCharactersDocuments.Select(x => GetMangaCharacter(x)));
        }

        private async Task<MangaCharacter> GetMangaCharacter(HtmlDocument htmlDocument)
        {
            var name = HttpUtility.HtmlDecode(htmlDocument.DocumentNode.SelectSingleNode("//h2[contains(@class,'normal_header')]").InnerText);
            var imageElement = htmlDocument.DocumentNode.SelectSingleNode("//div[not(@class)]/a/img");
            var imageUrl = imageElement == null ? DefaultValuesManager.DefaultNoImageImageUrl : imageElement.Attributes["data-src"].Value;
            var isMain = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(text(),'Mangaography')]/following::small[1]").InnerText == "Main";  
            var description = HttpUtility.HtmlDecode(htmlDocument.DocumentNode.SelectSingleNode("//td[@valign='top' and not(@class)]")
                .GetDirectInnerText().Replace("\n", "").Replace("\t", ""));

            return new MangaCharacter(name, imageUrl, isMain, description);
        }

        private async Task<List<List<string>>> GetImagesUrlsForCharacters(IEnumerable<string> characterImagesLinks)
        {
            var allImages = new List<List<string>>();

            var imagesRequests = characterImagesLinks.Select(x => RequestBuilder.CreateRequest()
                    .SetRequestResource(x.Replace(MyAnimeListUrl, string.Empty)).GetRequest());
            var htmlCharactersDocuments = await Task.WhenAll(imagesRequests.Select(x => RequestExecutor.SendRequestAsync(x)));

            foreach (var characterDocument in htmlCharactersDocuments)
            {
                var imagesForCharacter = new List<string>();
                var imageElement = characterDocument.DocumentNode.SelectNodes("//img[contains(@class,'portrait')]");
                if (imageElement != null)
                    imagesForCharacter.AddRange(imageElement.Select(x => x.Attributes["data-src"].Value));
                if (imagesForCharacter.Count > 0)
                    imagesForCharacter.RemoveAt(0);
                allImages.Add(imagesForCharacter);
            }

            return allImages;
        }
    }
}
