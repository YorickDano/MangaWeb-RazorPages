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
        private const string MyAnimeListApiUrl = "https://api.myanimelist.net";
        private RestClient _restClient;
        private RequestBuilder _requestBuilder;
        private readonly MangaCharacterClient _mangaCharacterClient;

        public ResearchMangaClient(MangaCharacterClient mangaCharacterClient)
        {
            _restClient = new RestClient(MyAnimeListApiUrl);
            _requestBuilder = new RequestBuilder();
            _mangaCharacterClient = mangaCharacterClient;
        }

        public async Task<Manga> GetManga(string title, IEnumerable<string> nameTitlesExists)
        {
            var mangaUrlInfoRequest = _requestBuilder.CreateRequest()
                .SetRequestResource("/v2/manga")
                .AddRequestParameter("q", title)
                .GetRequest();
            var response = await _restClient.ExecuteAsync(mangaUrlInfoRequest);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            var deserializeUrlResponse = JsonConvert.DeserializeObject<Root>(response.Content);
            var manga = deserializeUrlResponse.data.FirstOrDefault();
            if (manga == null || nameTitlesExists.Contains(manga.node.title))
            {
                return null;
            }

            var mangaInfoRequest = _requestBuilder.CreateRequest()
                .SetRequestResource($"/v2/manga/{manga.node.id}")
                .AddRequestParameter("fields", "title main_picture start_date end_date synopsis mean rank popularity genres created_at media_type status num_volumes num_chapters authors{first_name,last_name}")
                .GetRequest();
            var deserializeInfoResponse = JsonConvert.DeserializeObject<MangaInfoModel.Root>((await _restClient.ExecuteAsync(mangaInfoRequest)).Content);
            var mangaLinksTask = new MangaReadLinksClient().FindLinksForAsync(title);
            DateTime startDate, endDate;
            var isStart = DateTime.TryParse(deserializeInfoResponse.start_date, out startDate);
            var isEnd = DateTime.TryParse(deserializeInfoResponse.end_date, out endDate);
            startDate = isStart ? startDate : new DateTime(1900,1,1);
            endDate = isEnd ? endDate : new DateTime(1901,1,1);

            var mangaObj = new Manga
            {
                OriginTitle = deserializeInfoResponse.title,
                MangaImageUrl = deserializeInfoResponse.main_picture.large,
                Description = deserializeInfoResponse.synopsis,
                Status = deserializeInfoResponse.status == "finished" ? MangaStatus.Finished : MangaStatus.Publishing,
                Score = (float)deserializeInfoResponse.mean,
                Language = Language.en,
                CountOfChapters = deserializeInfoResponse.num_chapters == 0 ? -1 : deserializeInfoResponse.num_chapters,
                CountOfVolume = deserializeInfoResponse.num_volumes == 0 ? -1 : deserializeInfoResponse.num_volumes,
                Popularity = deserializeInfoResponse.popularity,
                Ranked = deserializeInfoResponse.rank,
                Published = $"{startDate.ToShortDateString()} - {(endDate.Year < 1900 ? "Unknown" : endDate.ToShortDateString())}",
                Genres = deserializeInfoResponse.genres.Select(x => x.name),
                YearOfIssue = Convert.ToInt32(deserializeInfoResponse.start_date.Remove(4)),
                Type = char.ToUpper(deserializeInfoResponse.media_type[0]) + deserializeInfoResponse.media_type.Substring(1).Replace('_', ' '),
                Authors = deserializeInfoResponse.authors.Select(x => string.Concat(x.node.first_name, " ", x.node.last_name))
            };

            var charactersTask = _mangaCharacterClient.GetAllCharacters(deserializeInfoResponse.title, mangaObj, deserializeInfoResponse.id);

            await Task.WhenAll(charactersTask, mangaLinksTask);

            mangaObj.ReadLinks = await mangaLinksTask;

            return mangaObj;
        }

        public async Task<Manga> UpdateMangaAsync(Manga currentManga, IEnumerable<string> mangaTitlesExists)
        {
            return await GetManga(currentManga.OriginTitle, mangaTitlesExists);
        }
    }
}
