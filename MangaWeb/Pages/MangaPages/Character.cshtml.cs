using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MangaWeb.Pages.MangaPages
{
    public class CharacterModel : PageModel
    {
        private readonly MangaWebContext _context;
        public MangaCharacter MangaCharacter;
        
        public CharacterModel(MangaWebContext mangaWebContext)
        {
            _context = mangaWebContext;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            MangaCharacter = await _context.MangaCharacter.Where(x => x.Id == id).FirstOrDefaultAsync();

            return Page();
        }
    }
}
