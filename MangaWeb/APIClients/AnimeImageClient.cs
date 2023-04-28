using HtmlAgilityPack;
using System.Buffers.Text;
using System.Net;

namespace MangaWeb.APIClients
{
    public class AnimeImageClient : RestClientApi
    {
        private readonly string[] RandomAnimeCharactersForProfileImage = {"Lucoa", "Ilulu", "Tohru", "Kanna Kobayashi",
            "Elma Joui", "Albedo Overlord", "Aura Bella Fiora", "Entoma Vasilissa Zeta","Lupusregina Beta",
            "Shalltear Bloodfallen","Hitagi Senjougahara","Shinobu Oshino","Yotsugi Ononoki","Chika Fujiwara",
            "Kaguya Shinomiya","Aqua Konosuba","Lalatina Dustiness Ford","Megumin","Wiz Konosuba","Sylvia Konosuba" };

        public async Task<string> GetRandomImageAsString(AnimeType animeType = AnimeType.Casual)
        {
            var randomTopic = GetRandomAnimeCharacterTopic();
            var imagesLinksList = await GetImagesUrlsByTitle(randomTopic);
            var imageBytes = await WebClient
                .DownloadDataTaskAsync(GetRandomImageUrlFormList(imagesLinksList));
            var base64 = Convert.ToBase64String(imageBytes);
            var imgSrc = String.Format("data:image/jpg;base64,{0}", base64);

            return imgSrc;
        }

        public async Task<List<string>> GetImagesUrlsByTitle(string title, AnimeType animeType = AnimeType.Casual)
        {
            var links = new List<string>();
            var responseContent = animeType == AnimeType.Hentai
                ? await GetImageSearchRespounseContent(title + " hentai") : await GetImageSearchRespounseContent(title + " manga");
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(responseContent);
            links.AddRange(htmlDocument.DocumentNode.SelectNodes("//img[contains(@src,'https://encrypted')]").Select(x => x.GetAttributeValue("src", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRJ583o5BdMGzJadrs2jYP8GLQvssf37P1gJuJ4v8YShA6addxV8Y0uwcr-0ao&amp;s")));

            return links;
        }

        private string GetRandomAnimeCharacterTopic() => RandomAnimeCharactersForProfileImage[new Random()
          .Next(0, RandomAnimeCharactersForProfileImage.Length)];
        private string GetRandomImageUrlFormList(List<string> imagesLinks) =>
            imagesLinks[new Random().Next(0, imagesLinks.Count)];
    }

    public enum AnimeType
    {
        Casual,
        Hentai
    }
}
