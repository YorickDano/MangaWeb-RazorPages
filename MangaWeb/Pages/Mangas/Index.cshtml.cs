
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MangaWeb.Pages.Manga_s_
{
    public class IndexModel : PageModel
    {
        private readonly MangaWebContext _context;

        public MangaWebUser? MangaWebUser { get; set; } = default!;

        public IndexModel(MangaWebContext context)
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

        }

        public List<Manga>? Manga { get; set; } = default!;
        public List<FullManga> FullManga { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? GenersSelectedList { get; set; }

        public List<string>? Genres { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? MangaGenre { get; set; }

        public IEnumerable<SelectListItem> OrderOptions;

        public OrderInputModel? Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var manga = _context.Manga.Select(x => x);
            FullManga = await _context.FullManga.Select(x => x).ToListAsync();
            if (!string.IsNullOrEmpty(SearchString))
            {
                manga = manga.Where(s => s.Title.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(MangaGenre))
            {
                manga = manga.Where(x => x.Genre.Contains(MangaGenre));
            }

            if (Genres == null)
            {
                var allGenres = string.Join(",", _context.FullManga.Select(x => string.Join(",", x.Geners))).Split(',').Distinct();
                Genres = new List<string>(allGenres);
                GenersSelectedList = new SelectList(allGenres);
            }

            Manga = await manga.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteFullManga(int id)
        {
            var manga = await _context.FullManga.FirstOrDefaultAsync(x => x.Id == id);

            _context.FullManga.Remove(manga);
            await _context.SaveChangesAsync();
            FullManga = await _context.FullManga.Select(x => x).ToListAsync();
            Manga = await _context.Manga.Select(x => x).ToListAsync();
            return Page();
        }



        public async Task<IActionResult> OnGetOrderAsync(OrderInputModel input)
        {
            FullManga = await _context.FullManga.Select(x => x).ToListAsync();
            await OrderByOption(input.Option);
             OrderByGenrse(input.Geners);
             OrderByYear(input.YearFrom, input.YearTo);
             OrderByScore(input.ScoreFrom, input.ScoreTo);
            OrderByCountOfChapters(input.CountOfChaptersFrom, input.CountOfChaptersTo);
            Manga = await _context.Manga.Select(x => x).ToListAsync();
            var allGenres = string.Join(",", _context.FullManga.Select(x => string.Join(",", x.Geners))).Split(',').Distinct();
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
                        FullManga = await _context.FullManga.Select(x => x).OrderBy(x => x.OriginTitle).ToListAsync();
                        break;
                    }
                case "Score":
                    {
                        FullManga = await _context.FullManga.Select(x => x).OrderBy(x => x.Score).Reverse().ToListAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void OrderByGenrse(string[]? gener)
        {
            if (!(gener is null) && gener.Any())
            {
                FullManga = FullManga.Where(x => x.Geners.Any(y => gener.Contains(y))).ToList();
            }
        }

        private void OrderByYear(int? yearFrom, int? yearTo)
        {

            if (yearFrom.HasValue)
            {
                if (yearTo.HasValue)
                {
                    FullManga = FullManga.Where(x => x.YearOfIssue >= yearFrom && x.YearOfIssue <= yearTo).ToList();
                    return;
                }
                FullManga = FullManga.Where(x => x.YearOfIssue >= yearFrom).ToList();
                return;
            }
            if (yearTo.HasValue)
            {
                FullManga = FullManga.Where(x => x.YearOfIssue <= yearTo).ToList();
            }
        }
        private async Task OrderByTwo<T>(T? from, T? to)
        {

        }

        private void OrderByScore(double? scoreFrom, double? scoreTo)
        {

            if (scoreFrom.HasValue)
            {
                if (scoreTo.HasValue)
                {
                    FullManga = FullManga.Where(x => x.Score >= scoreFrom && x.Score <= scoreTo).ToList();
                    return;
                }
                FullManga = FullManga.Where(x => x.Score >= scoreFrom).ToList();
                return;
            }
            if (scoreTo.HasValue)
            {
                FullManga = FullManga.Where(x => x.Score <= scoreTo).ToList();
            }
        }

        private void OrderByCountOfChapters(double? countOfChaptersFrom, double? countOfChaptersTo)
        {

            if (countOfChaptersFrom.HasValue)
            {
                if (countOfChaptersTo.HasValue)
                {
                    FullManga = FullManga.Where(x => x.CountOfChapters >= countOfChaptersFrom && x.CountOfChapters <= countOfChaptersTo).ToList();
                    return;
                }
                FullManga = FullManga.Where(x => x.CountOfChapters >= countOfChaptersFrom).ToList();
                return;
            }
            if (countOfChaptersTo.HasValue)
            {
                FullManga = FullManga.Where(x => x.CountOfChapters <= countOfChaptersTo).ToList();
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
        }
    }
}
