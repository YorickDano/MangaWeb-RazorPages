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
        private protected const string MyAnimeListUrl = "https://myanimelist.net";
        private protected const string MyAnimeListApiUrl = "https://api.myanimelist.net";
        private protected RestClient RestClient  =new RestClient(MyAnimeListUrl);
        private protected RequestBuilder RequestBuilder = new RequestBuilder();

        public async Task<Manga> GetAllCharacters(string title, Manga manga, int id)
        {
            var htmlDocument = new HtmlDocument();
            var requestCharactersList = RequestBuilder.CreateRequest().SetRequestResource($"manga/{id}/{title}/characters").GetRequest();
            var responseCharactersList = await RestClient.ExecuteAsync(requestCharactersList);
            
            htmlDocument.LoadHtml(responseCharactersList.Content);
            
            var charactersNodes = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'picSurround')]/a");
            if(charactersNodes == null)
            {
                return manga;
            }
            var charactersLinks = charactersNodes.Select(x => x.Attributes["href"].Value); 

            var responsesCharactersPages = new List<RestResponse>();
            var requests = charactersLinks.Select(x => RequestBuilder.CreateRequest()
            .SetRequestResource(x.Replace(MyAnimeListUrl, String.Empty)).GetRequest());
            var requestExecutor = new RequestExecutor(MyAnimeListUrl);

            var htmlCharactersDocuments = await Task.WhenAll(requests.Select(x => requestExecutor.SendRequestAsync(x)));
            manga.Characters.AddRange(htmlCharactersDocuments.Select(x => GetMangaCharacter(x).Result));

            var imagesUrlsForCharacters = await GetImagesUrlsForCharacters(charactersLinks.Select(x => x + "/pics"));

            for (var i = 0; i < manga.Characters.Count; ++i)
            {
                imagesUrlsForCharacters[i].Remove(manga.Characters[i].ImageUrl);
                manga.Characters[i].ImagesUrls = imagesUrlsForCharacters[i];
            }
            htmlCharactersDocuments = null;
            return manga;
        }
        private async Task<IEnumerable<MangaCharacter>> GetMangaCharacters(IEnumerable<HtmlDocument> htmlDocuments)
        {
            var characters = new List<MangaCharacter>();
            foreach(var htmlDocument in htmlDocuments)
            {
                characters.Add(await GetMangaCharacter(htmlDocument));
            }
            return characters;
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

            var requestExecutor = new RequestExecutor(MyAnimeListUrl);
            var imagesRequests = characterImagesLinks.Select(x => RequestBuilder.CreateRequest()
                    .SetRequestResource(x.Replace(MyAnimeListUrl, string.Empty)).GetRequest());
            var htmlCharactersDocuments = await Task.WhenAll(imagesRequests.Select(x => requestExecutor.SendRequestAsync(x)));

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

            htmlCharactersDocuments = null;
            return allImages;
        }
    }
}
