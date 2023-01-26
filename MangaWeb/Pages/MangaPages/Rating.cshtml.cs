using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages.MangaPages
{
    public class RatingModel : PageModel
    {
        private readonly MangaWebContext _context;

        public IList<Manga> TopScoreManga { get; private set; }
        public IList<Manga> TopGenreManga { get; private set; }
        public IList<string> Genres { get; private set; }

        public readonly IStringLocalizer<SharedResource> Localizer;

        public RatingModel(MangaWebContext context, 
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            Localizer = localizer;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            TopScoreManga = _context.Manga.Select(x => x).OrderByDescending(x => x.Score).Take(5).ToList();
            var allGenres = string.Join(",", _context.Manga.Select(x => string.Join(",", x.Genres))).Split(',').Distinct();
            Genres = new List<string>(allGenres);
            return Page();
        }

        public async Task<IActionResult> OnGetTopMangaByGenres(string[] genres)
        {
            TopGenreManga = _context.Manga.ToList().Where(x => genres.All(y => x.Genres.Contains(y)))
                .OrderByDescending(x => x.Score).ToList();

            return await OnGetAsync();
        }
    }
}
