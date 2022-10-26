using MangaWeb.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MangaWeb.Pages.ExpandedManga
{
    public class ExpandedMangaModel : PageModel
    {
        private readonly MangaWebContext _context;

        public ExpandedMangaModel(MangaWebContext context)
        {
            _context = context;
        }

        public FullManga? FullManga { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            FullManga = await _context.FullManga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            if (FullManga is null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetMangaDeletionAsync(int? id)
        {
            FullManga = await _context.FullManga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);
            var charactersOfMaga = _context.MangaCharacter.Where(x => x.FullManga.Id == id);

            foreach (var character in charactersOfMaga)
            {
                _context.MangaCharacter.Remove(character);
            }
            _context.FullManga.Remove(FullManga);
            await _context.SaveChangesAsync();

            return Redirect("../Index");
        }
    }
}
