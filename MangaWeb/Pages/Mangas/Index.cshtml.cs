
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MangaWeb.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MangaWeb.APIClient;
using Microsoft.AspNetCore.Identity;
using MangaWeb.Areas.Identity.Data;

namespace MangaWeb.Pages.Manga_s_
{
    public class IndexModel : PageModel
    {
        private readonly MangaWebContext _context;

        public MangaWebUser? MangaWebUser { get; set; } = default!;

        public IndexModel(MangaWebContext context)
        {
            _context = context;
        }

        public List<Manga> Manga { get; set; } = default!;
        public List<FullManga> FullManga { get; set; } = default!;
        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }
        public SelectList? Genres { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? MangaGenre { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            var allGenres = string.Join("\t", _context.Manga.Select(x => x.Genre));
            var manga = _context.Manga.Select(x => x);
            FullManga = await _context.FullMangas.Select(x => x).ToListAsync();
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
                Genres = new SelectList(allGenres.Split('\t').Distinct());
            }

            Manga = await manga.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteFullManga(int id)
        {
            var manga = await _context.FullMangas.FirstOrDefaultAsync(x => x.Id == id);

            _context.FullMangas.Remove(manga);
            await _context.SaveChangesAsync();
            FullManga = await _context.FullMangas.Select(x => x).ToListAsync();
            Manga = await _context.Manga.Select(x => x).ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnGetSortBy()
        {
            switch (SortByOption) 
            {
                case "Score":
                    {
                        FullManga = await _context.FullMangas.Select(x => x).OrderBy(x => x.Score).ToListAsync();
                        break;
                    }
                default:
                    {
                        break;
                    }      
            }
            Manga = await _context.Manga.Select(x => x).ToListAsync();
            return Page();
        }
    }
}
