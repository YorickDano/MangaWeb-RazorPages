using HtmlAgilityPack;
using MangaWeb.APIClients.Models;
using MangaWeb.APIClients.Services;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using static MangaWeb.APIClients.Models.MangaUrlInfoModel;

namespace MangaWeb.APIClients
{
    public class ResearchMangaClient
    {
        private protected const string MyAnimeListUrl = "https://myanimelist.net";
        private protected const string MyAnimeListApiUrl = "https://api.myanimelist.net";
        private protected RestClient RestClient;
        private protected RequestBuilder RequestBuilder;
        private readonly MangaCharacterClient _mangaCharacterClient;

        private Manga Manga;

        public ResearchMangaClient(MangaCharacterClient mangaCharacterClient)
        {
            RestClient = new RestClient(MyAnimeListApiUrl);
            RequestBuilder = new RequestBuilder();
            _mangaCharacterClient = mangaCharacterClient;
        }

        public async Task<Manga> GetFullManga(string title)
        {
            Manga = Manga.CreateNew();
            var mangaUrlInfoRequest = RequestBuilder.CreateRequest().SetRequestResource("/v2/manga")
                .AddRequestParameter("q", title).GetRequest();
            var response = (await RestClient.ExecuteAsync(mangaUrlInfoRequest)).Content;
            var desiralizeUrlResponse = JsonConvert.DeserializeObject<Root>(response);
            var mangaId = desiralizeUrlResponse.data[0].node.id;
            var mangaInfoRequest = RequestBuilder.CreateRequest().SetRequestResource($"/v2/manga/{mangaId}")
                .AddRequestParameter("fields", "title main_picture start_date end_date synopsis mean" +
                " rank popularity genres created_at media_type status num_volumes num_chapters authors").GetRequest();
            var desiralizeInfoResponse = JsonConvert.DeserializeObject<MangaInfoModel.Root>((await RestClient.ExecuteAsync(mangaInfoRequest)).Content);
            var mangaInfoTask = SetMangaInfoFromApiAsync(desiralizeInfoResponse);
            var charactersTask = SetMangaCharacters(Manga, title);
            var mangaAutorsTask = SetMangaAutorsAsync(desiralizeInfoResponse.id);
            var mangaLinksTask = new MangaReadLinksClient().FindLinksForAsync(title);
            Task.WaitAll(mangaInfoTask, charactersTask, mangaAutorsTask, mangaLinksTask);

            Manga.ReadLinks = mangaLinksTask.Result;

            RestClient.Dispose();
            return Manga;
        }

        public async Task SetMangaInfoFromApiAsync(MangaInfoModel.Root info)
        {
            Manga.OriginTitle = info.title;
            Manga.MangaImageUrl = info.main_picture.large;
            Manga.Description = info.synopsis;
            switch (info.status)
            {
                case "finished":
                    {
                        Manga.Status = MangaStatus.Finished;
                        break;
                    }
                default:
                    {
                        Manga.Status = MangaStatus.Publishing;
                        break;
                    }
            }
            Manga.Score = (float)info.mean;
            Manga.Language = Language.en;
            Manga.CountOfChapters = info.num_chapters == 0 ? -1 : info.num_chapters;
            Manga.CountOfVolume = info.num_volumes == 0 ? -1 : info.num_volumes;
            Manga.Popularity = info.popularity;
            Manga.Ranked = info.rank;
            Manga.Published = (Convert.ToDateTime(info.start_date)).ToShortDateString() + $" - {(info.end_date.Year < 1900 ? "Unknown" : info.end_date.ToShortDateString())}";
            Manga.Genres = info.genres.Select(x => x.name);
            Manga.YearOfIssue = Convert.ToInt32(info.start_date.Remove(4));
            Manga.Type = char.ToUpper(info.media_type[0]) + info.media_type.Substring(1).Replace('_',' ');

        }
        public async Task SetMangaAutorsAsync(int mangaId)
        {
            var RestClient = new RestClient(MyAnimeListUrl);

            var request = RequestBuilder.CreateRequest().SetRequestResource($"/manga/{mangaId}").GetRequest();

            var response = await RestClient.ExecuteAsync(request);
            var htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(response.Content);
            Manga.Authors = new List<string>(htmlDocument.DocumentNode
                .SelectNodes("//span[contains(text(),'Authors')]/../a")
                .Select(x => x.GetDirectInnerText().Replace(",", string.Empty)));
            RestClient.Dispose();
        }
        public async Task<Manga> UpdateMangaAsync(Manga currentManga)
        {
            return await GetFullManga(currentManga.OriginTitle);
        }

        protected async Task<string> GetManagaUrl(string title)
        {
            var htmlDocument = await GetHtmlOfMangaSearchFromMyAnimeListAsync(title);
            var mangaLink = htmlDocument.DocumentNode.SelectSingleNode("//a[@class='hoverinfo_trigger']")
                .GetAttributeValue("href", DefaultValuesManager.DefaultMangaLink);

            return mangaLink.Replace(MyAnimeListUrl, "");
        }

        protected async Task<HtmlDocument> GetHtmlOfMangaSearchFromMyAnimeListAsync(string title)
        {
            var htmlDocument = new HtmlDocument();
            var request = RequestBuilder.CreateRequest()
              .SetRequestResource("/manga.php")
              .AddRequestParameter("q", title)
              .AddRequestParameter("cat", "manga")
              .GetRequest();
            RestClient = new RestClient(MyAnimeListUrl);
            var mangaSearchSiteResponse = RestClient.Execute(request);
            htmlDocument.LoadHtml(mangaSearchSiteResponse.Content);
            return htmlDocument;
        }

        private async Task<Manga> SetTitles(HtmlDocument htmlDocument)
        {
            var titlesElement = htmlDocument.DocumentNode.SelectSingleNode("//h1//span[@itemprop='name']");
            Manga.OriginTitle = titlesElement.GetDirectInnerText();
            return Manga;
        }

        private async Task<Manga> SetMangaImage(HtmlDocument htmlDocument)
        {
            Manga.MangaImageUrl = htmlDocument.DocumentNode
                .SelectSingleNode("//img[@itemprop='image']")
                .GetAttributeValue("data-src", DefaultValuesManager.DefaultMangaImageUrl);

            return Manga;
        }

        private async Task<Manga> SetDescription(HtmlDocument htmlDocument)
        {
            var fullDescription = htmlDocument.DocumentNode
                .SelectSingleNode("//span[@itemprop='description']").GetDirectInnerText().Replace("&#039;s", "");
            Manga.Description = fullDescription.Remove(fullDescription.LastIndexOf('.') + 1);
            return Manga;
        }
        private async Task<Manga> SetMangaInfo(HtmlDocument htmlDocument)
        {
            var documentNode = htmlDocument.DocumentNode;
            Manga.CountOfVolume = int.TryParse
                (documentNode.SelectSingleNode("//span[contains(text(),'Volume')]/..")
                .GetDirectInnerText().Trim(), out DefaultNumber) ? DefaultNumber : -1;
            Manga.CountOfChapters = int.TryParse
                (documentNode.SelectSingleNode("//span[contains(text(),'Chapters')]/..")
                .GetDirectInnerText().Trim(), out DefaultNumber) ? DefaultNumber : -1;
            Manga.Status = (MangaStatus)Enum.Parse(typeof(MangaStatus), documentNode.SelectSingleNode("//span[contains(text(),'Status')]/..").GetDirectInnerText().Trim());
            Manga.Published = documentNode.SelectSingleNode("//span[contains(text(),'Published')]/..").GetDirectInnerText().Trim();
            var yearStr = Manga.Published.Split(' ')[3];
            var yearTry = 0;
            Manga.YearOfIssue = int.TryParse(yearStr, out yearTry) ? yearTry : 0;
            Manga.Genres = new List<string>(documentNode.SelectNodes("//span[@itemprop='genre']").Select(x => x.GetDirectInnerText()));
            Manga.Authors = new List<string>(documentNode.SelectNodes("//span[contains(text(),'Authors')]/../a").Select(x => x.GetDirectInnerText().Replace(",", string.Empty)));
            Manga.Popularity = int.Parse(documentNode.SelectSingleNode("//span[contains(text(),'Popularity')]/..").GetDirectInnerText().Trim().TrimStart('#'));
            var rankedTry = 0;
            Manga.Ranked = int.TryParse(documentNode.SelectSingleNode("//span[contains(text(),'Ranked')]/..").GetDirectInnerText().Trim().TrimStart('#'), out rankedTry) ? rankedTry : -1;
            var scoreTry = 0.0f;
            Manga.Score = float.TryParse(documentNode.SelectSingleNode("//span[contains(text(),'Score')]/../span/span").GetDirectInnerText(),
                NumberStyles.Number, new NumberFormatInfo { NumberDecimalSeparator = "." }, out scoreTry) ? scoreTry : -1;

            return Manga;
        }

        private int DefaultNumber = -1;

        private async Task<Manga> SetMangaCharacters( Manga manga, string title)
        {
            var mangaName = await GetManagaUrl(title);
            return await _mangaCharacterClient.GetAllCharacters(manga.OriginTitle, manga, mangaName); ;
        }
    }
}
