using MangaWeb.Areas.Identity.Data;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Runtime.Serialization.Formatters.Binary;

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
                    Value = "Title",
                    Text = "Title"
                },
                new SelectListItem
                {
                    Value = "Score",
                    Text = "Score"
                }
            };
            Localizer = localizer;
            Ongoing = Localizer["Ongoing"];
            Released = Localizer["Released"];
        }

        public List<Manga> Manga { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? GenersSelectedList { get; set; }

        public List<string>? Genres { get; set; }
        public string Ongoing { get; set; }
        public string Released { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? MangaGenre { get; set; }

        public IEnumerable<SelectListItem> OrderOptions;

        public OrderInputModel? Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Manga = await _context.Manga.Select(x => x).ToListAsync();
            if (!string.IsNullOrEmpty(SearchString))
            {
                Manga = Manga.Where(s => s.OriginTitle.Contains(SearchString)).ToList();
            }

            if (!string.IsNullOrEmpty(MangaGenre))
            {
                Manga = Manga.Where(x => x.Genres.Contains(MangaGenre)).ToList();
            }

            if (Genres == null)
            {
                var allGenres = string.Join(",", _context.Manga.Select(x => string.Join(",", x.Genres))).Split(',').Distinct();
                Genres = new List<string>(allGenres);
                GenersSelectedList = new SelectList(allGenres);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Manga = await _context.Manga.ToListAsync();

            if (!string.IsNullOrEmpty(SearchString))
            {
                Manga = await _context.Manga.Where(x => x.OriginTitle!.Contains(SearchString)).ToListAsync();
            }

            var allGenres = string.Join(",", _context.Manga.Select(x => string.Join(",", x.Genres))).Split(',').Distinct();
            Genres = new List<string>(allGenres);
            GenersSelectedList = new SelectList(allGenres);

            return Page();
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



        public async Task<IActionResult> OnGetOrderAsync(OrderInputModel input)
        {
            Manga = await _context.Manga.Select(x => x).ToListAsync();
            await OrderByOption(input.Option);
            OrderByGenrse(input.Geners);
            OrderByYear(input.YearFrom, input.YearTo);
            OrderByScore(input.ScoreFrom, input.ScoreTo);
            OrderByCountOfChapters(input.CountOfChaptersFrom, input.CountOfChaptersTo);
            OrderByStatus(input.Status);
            var allGenres = string.Join(",", _context.Manga.Select(x => string.Join(",", x.Genres))).Split(',').Distinct();
            Genres = new List<string>(allGenres);
            GenersSelectedList = new SelectList(allGenres);
            return Page();
        }

        private async Task OrderByOption(string? option)
        {
            switch (option)
            {
                case "Title":
                    {
                        Manga = await _context.Manga.Select(x => x).OrderBy(x => x.OriginTitle).ToListAsync();
                        break;
                    }
                case "Score":
                    {
                        Manga = await _context.Manga.Select(x => x).OrderBy(x => x.Score).Reverse().ToListAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void OrderByGenrse(string[]? geners)
        {
            if (!(geners is null) && geners.Any())
            {
                Manga = Manga.Where(x => x.Genres.Where(y => geners.Contains(y)).Count() == geners.Length).ToList();
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
                Manga = Manga.Where(x => status == "Ongoing" 
                ? (x.Status == MangaStatus.Publishing || x.Status == MangaStatus.Выходит) 
                : (x.Status == MangaStatus.Finished || x.Status == MangaStatus.Издано)).ToList();
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
        }
    }
}
