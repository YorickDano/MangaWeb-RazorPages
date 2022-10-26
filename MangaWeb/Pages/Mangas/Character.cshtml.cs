using MangaWeb.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MangaWeb.Pages.Mangas
{
    public class CharacterModel : PageModel
    {
        private readonly MangaWebContext _context;
        public List<MangaCharacter> MangaCharacters;
        
        public CharacterModel(MangaWebContext mangaWebContext)
        {
            _context = mangaWebContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            MangaCharacters = await _context.MangaCharacter.Where(x => x.Id == id).ToListAsync();

            return Page();
        }
    }
}
