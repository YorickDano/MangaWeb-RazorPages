using HtmlAgilityPack;
using MangaWeb.APIClients.Services;
using MangaWeb.Managers;
using MangaWeb.Models;
using RestSharp;
using System.Web;

namespace MangaWeb.APIClients
{
    public class MangaCharacterClient
    {
        private const string MyAnimeListUrl = "https://myanimelist.net";
        private readonly RestClient _restClient;
        private readonly RequestBuilder _requestBuilder;

        public MangaCharacterClient()
        {
            _restClient = new RestClient(MyAnimeListUrl);
            _requestBuilder = new RequestBuilder();
        }

        public async Task<Manga> GetAllCharacters(string title, Manga manga, int id)
        {
            var charactersLinks = await GetCharacterLinks(id, title);
            if(charactersLinks == null)
            {
                return manga;
            }
            var characterDetailsTasks = charactersLinks.Select(GetCharacterDetailsAsync).ToList();
            var imagesUrlsTasks = charactersLinks.Select(link => GetImagesUrlsForCharacterAsync(link + "/pics")).ToList();

            var characterDetails = await Task.WhenAll(characterDetailsTasks);
            var imagesUrls = await Task.WhenAll(imagesUrlsTasks);
            manga.Characters = new List<MangaCharacter>();
            for (var i = 0; i < characterDetails.Length; i++)
            {
                var character = characterDetails[i];
                character.ImagesUrls = imagesUrls[i];
                manga.Characters.Add(character);
            }

            return manga;
        }

        private async Task<List<string>> GetCharacterLinks(int id, string title)
        {
            var request = _requestBuilder.CreateRequest()
                .SetRequestResource($"manga/{id}/{title}/characters").GetRequest();
            var response = await _restClient.ExecuteAsync(request);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.Content);

            var characterLinks = htmlDocument.DocumentNode
                .SelectNodes("//div[contains(@class,'picSurround')]/a")?
                .Select(node => node.Attributes["href"].Value)
                .ToList() ?? new List<string>();

            return characterLinks;
        }

        private async Task<MangaCharacter> GetCharacterDetailsAsync(string characterLink)
        {
            var request = _requestBuilder.CreateRequest()
                .SetRequestResource(characterLink.Replace(MyAnimeListUrl, string.Empty)).GetRequest();
            var response = await _restClient.ExecuteAsync(request);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.Content);

            var name = HttpUtility.HtmlDecode(htmlDocument.DocumentNode
                .SelectSingleNode("//h2[contains(@class,'normal_header')]").InnerText);

            var imageElement = htmlDocument.DocumentNode.SelectSingleNode("//div[not(@class)]/a/img");
            var imageUrl = imageElement?.Attributes["data-src"].Value ?? DefaultValuesManager.DefaultNoImageImageUrl;

            var isMain = htmlDocument.DocumentNode.SelectSingleNode("//div[contains(text(),'Mangaography')]/following::small[1]").InnerText == "Main";

            var description = HttpUtility.HtmlDecode(htmlDocument.DocumentNode
                .SelectSingleNode("//td[@valign='top' and not(@class)]")
                .GetDirectInnerText().Replace("\n", "").Replace("\t", ""));

            return new MangaCharacter(name, imageUrl, isMain, description);
        }

        private async Task<List<string>> GetImagesUrlsForCharacterAsync(string characterImagesLink)
        {
            var htmlDocument = await GetHtmlDocumentAsync(characterImagesLink);
            var imagesForCharacter = new List<string>();
            var imageElements = htmlDocument.DocumentNode.SelectNodes("//img[contains(@class,'portrait')]");
            if (imageElements != null && imageElements.Count > 0)
            {
                imagesForCharacter?.AddRange(imageElements.Select(x => x.Attributes["data-src"].Value));
            }

            return imagesForCharacter;
        }

        private async Task<HtmlDocument> GetHtmlDocumentAsync(string url)
        {
            var request = _requestBuilder.CreateRequest().SetRequestResource(url.Replace(MyAnimeListUrl, string.Empty)).GetRequest();
            var response = await _restClient.ExecuteAsync(request);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.Content);

            return htmlDocument;
        }
    }
}
