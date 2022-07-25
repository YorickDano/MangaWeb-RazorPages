
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MangaWeb.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MangaWeb.Pages.Manga_s_
{
    public class IndexModel : PageModel
    {
        private readonly MangaWebContext _context;

        public IndexModel(MangaWebContext context)
        {
            _context = context;
        }

        public IList<Manga> Manga { get;set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public SelectList? Genres { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? MangaGenre { get; set; }

        public async Task<IActionResult> OnGetAsync()
        { 
            var allGenres = string.Join("\t", _context.Manga.Select(x => x.Genre));
            var manga = _context.Manga.Select(x => x);

            if (!string.IsNullOrEmpty(SearchString))
            {
                manga = manga.Where(s => s.Title.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(MangaGenre))
            {
                manga = manga.Where(x => x.Genre.Contains(MangaGenre));
            }

            Genres = new SelectList(allGenres.Split('\t').Distinct());
            Manga = await manga.ToListAsync();

            return Page();
        }
    }
}
