using HtmlAgilityPack;
using MangaWeb.APIClient;
using MangaWeb.APIClient.Services;
using MangaWeb.Managers;
using RestSharp;

namespace MangaWeb.APIClients
{
    public class MangaCharacterClient : ReseachMangaClient
    {
        public async void GetAllCharacters(string title)
        {
            var htmlPage = await GetHtmlOfMangaAsync(title);
            var charactersLink = htmlPage.DocumentNode.SelectNodes("//a[contains(text(),'Characters')]")[1].GetAttributeValue("href","");
            RestClient.ChangeBaseUrlOn(charactersLink);
            var responseCharactersList = await RestClient.ExecuteAsync(RequestBuilder.Get);
            var htmlDocument = new HtmlDocument();
            htmlDocument.Load(responseCharactersList.Content);
            var charactersLinks = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class,'spaceit_pad')]/a")
                .Select(x=>x.GetAttributeValue("href", DefaultValuesManager.DefaultCharacterLink));
            var responsesCharactersPages = new List<RestResponse>();

            foreach(var characterLink in charactersLinks)
            {
                RestClient.ChangeBaseUrlOn(characterLink);
                responsesCharactersPages.Add(await RestClient.ExecuteAsync(RequestBuilder.Get));
            }
        }
    }
}
