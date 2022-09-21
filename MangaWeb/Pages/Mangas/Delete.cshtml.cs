using MangaWeb.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MangaWeb.Pages.Manga_s_
{
    public class DeleteModel : PageModel
    {
        private readonly MangaWebContext _context;

        public DeleteModel(MangaWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Manga Manga { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Manga == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga.FirstOrDefaultAsync(m => m.Id == id);

            if (manga == null)
            {
                return NotFound();
            }
            else
            {
                Manga = manga;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Manga == null)
            {
                return NotFound();
            }
            var manga = await _context.Manga.FindAsync(id);

            if (manga != null)
            {
                Manga = manga;
                _context.Manga.Remove(Manga);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
