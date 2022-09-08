using HtmlAgilityPack;
using MangaWeb.APIClient.Services;
using MangaWeb.Models;
using System.Globalization;

namespace MangaWeb.APIClient
{
    public class ReseachClient : RestClientApi
    {
        private FullManga FullManga { get; set; }

        public async Task<FullManga> GetFullManga(string title)
        {
            RestClient.ChangeBaseUrlOn($"https://myanimelist.net/manga.php?q={title}&cat=manga");
            var htmlDocument = new HtmlDocument();
            var mangaSearchSiteResponse = await RestClient.ExecuteAsync(
              requestBuilder.CreateRequest().GetRequest());
            htmlDocument.LoadHtml(mangaSearchSiteResponse.Content);
            var mangaLink = htmlDocument.DocumentNode?
                .SelectNodes("//a[contains(@href,'https://myanimelist.net/manga/')and@class='hoverinfo_trigger']")
                .FirstOrDefault().GetAttributeValue("href", "https://myanimelist.net/manga/80119/Kobayashi-san_Chi_no_Maid_Dragon?q=kobay&cat=manga");
             RestClient.ChangeBaseUrlOn(mangaLink);
            var mangaSiteResponse = await RestClient.ExecuteAsync(
              requestBuilder.CreateRequest().GetRequest());
            htmlDocument.LoadHtml(mangaSiteResponse.Content);
            FullManga = new FullManga();
            FullManga = await SetTitles(htmlDocument, FullManga);
            FullManga = await SetMangaImage(htmlDocument, FullManga);
            FullManga = await SetDescription(htmlDocument, FullManga);
            FullManga = await SetMangaInfo(htmlDocument, FullManga);
            FullManga = await SetMangaCharacters(htmlDocument, FullManga);

            return FullManga;
        }

        private async Task<FullManga> SetTitles(HtmlDocument htmlDocument, FullManga fullMangaInfo)
        {

            var titlesElement = htmlDocument.DocumentNode.SelectSingleNode("//h1//span[@itemprop='name']");
            fullMangaInfo.OriginTitle = titlesElement.GetDirectInnerText();
            return fullMangaInfo;
        }

        private async Task<FullManga> SetMangaImage(HtmlDocument htmlDocument, FullManga fullMangaInfo)
        {      
            fullMangaInfo.MangaImageUrl = htmlDocument.DocumentNode
                .SelectSingleNode("//img[@itemprop='image']")
                .GetAttributeValue("data-src", DefaultUrl);
            
            return fullMangaInfo;
        }

        private async Task<FullManga> SetDescription(HtmlDocument htmlDocument, FullManga fullMangaInfo)
        {
            var fullDescription = htmlDocument.DocumentNode
                .SelectSingleNode("//span[@itemprop='description']").GetDirectInnerText().Replace("&#039;s","") ;
            fullMangaInfo.Description = fullDescription.Remove(fullDescription.LastIndexOf('.')+1);
            return fullMangaInfo;
        }
        private async Task<FullManga> SetMangaInfo(HtmlDocument htmlDocument, FullManga fullMangaInfo)
        {
            var documentNode = htmlDocument.DocumentNode;
            fullMangaInfo.CountOfVolume = int.TryParse
                (documentNode.SelectSingleNode("//span[contains(text(),'Volume')]/..")
                .GetDirectInnerText().Trim(), out DefaultNumber) ? DefaultNumber : -1;
            fullMangaInfo.CountOfChapters = int.TryParse
                (documentNode.SelectSingleNode("//span[contains(text(),'Chapters')]/..")
                .GetDirectInnerText().Trim(), out DefaultNumber) ? DefaultNumber : -1;
            fullMangaInfo.Status = (MangaStatus)Enum.Parse(typeof(MangaStatus), documentNode.SelectSingleNode("//span[contains(text(),'Status')]/..").GetDirectInnerText().Trim());
            fullMangaInfo.Published = documentNode.SelectSingleNode("//span[contains(text(),'Published')]/..").GetDirectInnerText().Trim();
            fullMangaInfo.Geners = new List<string>( documentNode.SelectNodes("//span[@itemprop='genre']").Select(x=>x.GetDirectInnerText()));
            fullMangaInfo.Autors = new List<string>( documentNode.SelectNodes("//span[contains(text(),'Authors')]/../a").Select(x=>x.GetDirectInnerText().Replace(",",String.Empty)));
            fullMangaInfo.Popularity = int.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Popularity')]/..").GetDirectInnerText().Trim().TrimStart('#'));
            fullMangaInfo.Ranked = int.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Ranked')]/..").GetDirectInnerText().Trim().TrimStart('#'));
            fullMangaInfo.Score = double.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Score')]/../span/span").GetDirectInnerText(), new NumberFormatInfo { NumberDecimalSeparator = "." });

            return fullMangaInfo;
        }

        private readonly string DefaultUrl = "https://i.redd.it/0074y2gbvyj71.png";
        private int DefaultNumber = -1;

        private async Task<FullManga> SetMangaCharacters(HtmlDocument htmlDocument, FullManga fullMangaInfo)
        {
            fullMangaInfo.Characters = new List<MangaCharacter>();

            if(htmlDocument.DocumentNode.SelectNodes("//div[@class='picSurround']/a/img") != null) 
            {     
            var charactersImageUlrs = htmlDocument.DocumentNode.SelectNodes("//div[@class='picSurround']/a/img").Select(x=>x.GetAttributeValue("data-src", DefaultUrl).Replace("/r/42x62","")).ToArray();
            var charactersNames = htmlDocument.DocumentNode.SelectNodes("//td[@class='borderClass' and @valign='top']/a").Select(x => x.GetDirectInnerText().Replace(",", string.Empty)).ToArray();
            var charactersMain = htmlDocument.DocumentNode.SelectNodes("//td[@class='borderClass']/div/small").Select(x => x.GetDirectInnerText()).ToArray();
            
           
            for (int i = 0; i < charactersNames.Count(); ++i)
            {
                fullMangaInfo.Characters.Add(
                    new MangaCharacter(charactersNames[i], charactersImageUlrs[i], charactersMain[i].Equals("Main")));
            }
            }
            return fullMangaInfo;
        }
    }
}
