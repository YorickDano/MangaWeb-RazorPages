using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MangaWeb.Pages.MangaPages
{
    public class IndexModel : PageModel
    {
        private readonly MangaWebContext _context;

        public MangaWebUser? MangaWebUser { get; set; } = default!;
        public readonly IStringLocalizer<SharedResource> Localizer;

        public IndexModel(MangaWebContext context,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            OrderOptions = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = localizer["Title"],
                    Text = localizer["Title"]
                },
                new SelectListItem
                {
                    Value = localizer["Score"],
                    Text = localizer["Score"]
                }
            };
            Localizer = localizer;
            Ongoing = Localizer["Ongoing"];
            Released = Localizer["Released"];
            English = Localizer["English"];
            Russian = Localizer["Russian"];
            LoadMore = Localizer["LoadMore"];
        }

        public List<Manga> Manga { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? GenersSelectedList { get; set; }

        public List<string>? Genres { get; set; }
        public string Ongoing { get; set; } public string Released { get; set; }
        public string English { get; set; } public string Russian { get; set; }
        public string LoadMore { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? MangaGenre { get; set; }

        public IEnumerable<SelectListItem> OrderOptions;

        public OrderInputModel? Input { get; set; }

        public int MangaCount { get; private set; }

        private List<Manga> _constManga;

        public async Task<IActionResult> OnGetAsync()
        {
            IQueryable<Manga> mangaQuery = _context.Manga;
            _constManga = await mangaQuery.ToListAsync();
            HttpContext.Session.SetString("ConstManga", JsonConvert.SerializeObject(_constManga));
          
            if (!string.IsNullOrEmpty(SearchString))
            {
                mangaQuery = mangaQuery.Where(s => s.OriginTitle.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(MangaGenre))
            {
                mangaQuery = mangaQuery.Where(x => x.Genres.Contains(MangaGenre));
            }

            if (Genres == null)
            {
                var allGenres = string.Join(",", _context.Manga.Select(x => string.Join(",", x.Genres))).Split(',').Distinct();
                Genres = new List<string>(allGenres);
                GenersSelectedList = new SelectList(allGenres);
            }

            MangaCount = mangaQuery.Count();
            Manga = await mangaQuery.Take(28).ToListAsync();

            return Page();
        }
   
        public async Task<IActionResult> OnGetSearchAsync()
        {
            Manga = await _context.Manga.ToListAsync();

            if (!string.IsNullOrEmpty(SearchString))
            {
                Manga = await _context.Manga.Where(x => x.OriginTitle!.Contains(SearchString)).ToListAsync();
            }
            _constManga = new List<Manga>(Manga);
            HttpContext.Session.Remove("ConstManga");
            HttpContext.Session.SetInt32("MangaCount", Manga.Count);
            HttpContext.Session.SetString("ConstManga", JsonConvert.SerializeObject(_constManga));
            MangaCount = Manga.Count;
            Manga = Manga.Take(28).ToList();

            var allGenres = string.Join(",", _context.Manga.Select(x => string.Join(",", x.Genres))).Split(',').Distinct();
            Genres = new List<string>(allGenres);
            GenersSelectedList = new SelectList(allGenres);
            MangaCount = Manga.Count; 
            return Page();
        }

        public async Task<IActionResult> OnGetMangaCount()
        { 
            return new JsonResult(HttpContext.Session.GetInt32("MangaCount"));
        }

        public async Task<IActionResult> OnPostDeleteFullManga(int id)
        {
            var manga = await _context.Manga.FirstOrDefaultAsync(x => x.Id == id);
            var characters = _context.MangaCharacter.Where(x => x.Manga.Id == id);
            _context.Manga.Remove(manga);
            _context.MangaCharacter.RemoveRange(characters);
            await _context.SaveChangesAsync();
            Manga = await _context.Manga.Select(x => x).ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnGetLoadMore(int mangaCount)
        {
            var allGenres = string.Join(",", _context.Manga.Select(x => string.Join(",", x.Genres))).Split(',').Distinct();
            var constMangaJson = HttpContext.Session.GetString("ConstManga");

            _constManga = JsonConvert.DeserializeObject<List<Manga>>(constMangaJson);
            Manga = _constManga.Take(mangaCount).ToList();
            HttpContext.Session.Remove("MangaCount");
            HttpContext.Session.SetInt32("MangaCount", _constManga.Count);
            MangaCount = _constManga.Count;
            Genres = new List<string>(allGenres);
            GenersSelectedList = new SelectList(allGenres);

            return Partial("PartialViewes/_MangaListPartial", Manga);
        }

        public async Task<IActionResult> OnGetOrderAsync(string? SearchString, string? Option, string[]? Geners, int? CountOfChaptersFrom,
        int? CountOfChaptersTo, int? YearFrom, int? YearTo, double? ScoreFrom, double? ScoreTo,
        string Status, string Language)
        {
            Manga = _context.Manga.Select(x => x).ToList();
            if (!string.IsNullOrEmpty(SearchString))
            {
                Manga = await _context.Manga.Where(x => x.OriginTitle!.Contains(SearchString)).ToListAsync();
            }
            await OrderMangaAsync(Option, Geners, CountOfChaptersFrom, CountOfChaptersTo, 
                YearFrom, YearTo, ScoreFrom, ScoreTo, Status, Language);
            var allGenres = string.Join(",", _context.Manga.Select(x => string.Join(",", x.Genres))).Split(',').Distinct();
            Genres = new List<string>(allGenres);
            GenersSelectedList = new SelectList(allGenres);
            _constManga = new List<Manga>(Manga);
            HttpContext.Session.Remove("ConstManga");
            HttpContext.Session.SetInt32("MangaCount", Manga.Count);
            HttpContext.Session.SetString("ConstManga", JsonConvert.SerializeObject(_constManga));
            MangaCount = Manga.Count;
            Manga = Manga.Take(28).ToList();
           

            return Partial("PartialViewes/_MangaListPartial", Manga);
        }

        private async Task OrderMangaAsync(string? Option, string[]? Geners, int? CountOfChaptersFrom,
        int? CountOfChaptersTo, int? YearFrom, int? YearTo, double? ScoreFrom, double? ScoreTo,
        string Status, string Language)
        {
            Regex pattern = new("[\\'\"\\]\\[]");

            Geners = Geners.Select(x => pattern.Replace(x, "")).ToArray();
            if (string.IsNullOrEmpty(Geners[0])) Geners = null;
            await OrderByOption(Option);
            OrderByGenrse(Geners);
            OrderByYear(YearFrom, YearTo);
            OrderByScore(ScoreFrom, ScoreTo);
            OrderByCountOfChapters(CountOfChaptersFrom, CountOfChaptersTo);
            OrderByStatus(Status);
            OrderByLanguage(Language);          
        }

        private async Task OrderByOption(string? option)
        {
            var mangaQuery = _context.Manga.AsQueryable();

            switch (option)
            {
                case "Title":
                case "Название":
                    mangaQuery = mangaQuery.OrderBy(x => x.OriginTitle);
                    break;

                case "Score":
                case "Оценка":
                    mangaQuery = mangaQuery.OrderByDescending(x => x.Score);
                    break;

                default:
                    return;
            }

            Manga = await mangaQuery.ToListAsync();
        }

        private void OrderByGenrse(string[]? geners)
        {
            if (geners is not null && geners.Any())
            {
                Manga = Manga.Where(x => x.Genres.Any(y => geners[0].Contains(y))).ToList();
            }
        }

        private void OrderByYear(int? yearFrom, int? yearTo)
        {

            if (yearFrom.HasValue)
            {
                if (yearTo.HasValue)
                {
                    Manga = Manga.Where(x => x.YearOfIssue >= yearFrom && x.YearOfIssue <= yearTo).ToList();
                    return;
                }
                Manga = Manga.Where(x => x.YearOfIssue >= yearFrom).ToList();
                return;
            }
            if (yearTo.HasValue)
            {
                Manga = Manga.Where(x => x.YearOfIssue <= yearTo).ToList();
            }
        }
        private void OrderByScore(double? scoreFrom, double? scoreTo)
        {

            if (scoreFrom.HasValue)
            {
                if (scoreTo.HasValue)
                {
                    Manga = Manga.Where(x => x.Score >= scoreFrom && x.Score <= scoreTo).ToList();
                    return;
                }
                Manga = Manga.Where(x => x.Score >= scoreFrom).ToList();
                return;
            }
            if (scoreTo.HasValue)
            {
                Manga = Manga.Where(x => x.Score <= scoreTo).ToList();
            }
        }

        private void OrderByCountOfChapters(double? countOfChaptersFrom, double? countOfChaptersTo)
        {

            if (countOfChaptersFrom.HasValue)
            {
                if (countOfChaptersTo.HasValue)
                {
                    Manga = Manga.Where(x => x.CountOfChapters >= countOfChaptersFrom && x.CountOfChapters <= countOfChaptersTo).ToList();
                    return;
                }
                Manga = Manga.Where(x => x.CountOfChapters >= countOfChaptersFrom).ToList();
                return;
            }
            if (countOfChaptersTo.HasValue)
            {
                Manga = Manga.Where(x => x.CountOfChapters <= countOfChaptersTo).ToList();
            }
        }

        public void OrderByStatus(string status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                Manga = Manga.Where(x => status == "Ongoing" || status == "Выходит"
                ? (x.Status == MangaStatus.Publishing || x.Status == MangaStatus.Выходит) 
                : (x.Status == MangaStatus.Finished || x.Status == MangaStatus.Издано)).ToList();
            }
        }

        public void OrderByLanguage(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                Manga = Manga.Where(x => language == "English" || language == "Английский"
                ? x.Language == Language.en : x.Language == Language.ru).ToList();
            }
        }

        public class OrderInputModel
        {
            public string? Option { get; set; }
            public string[]? Geners { get; set; }
            public int? CountOfChaptersFrom { get; set; }
            public int? CountOfChaptersTo { get; set; }
            public int? YearFrom { get; set; }
            public int? YearTo { get; set; }
            public double? ScoreFrom { get; set; }
            public double? ScoreTo { get; set; }
            public string Status { get; set; }
            public string Language { get; set; }
        }
    }
}
