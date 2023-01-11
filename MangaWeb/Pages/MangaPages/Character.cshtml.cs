using MangaWeb.Areas.Identity.Data;
using MangaWeb.Managers;
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
        public UIValuesManager UIValuesManager;

        public CharacterModel(MangaWebContext mangaWebContext, UIValuesManager uIValuesManager)
        {
            _context = mangaWebContext;
            UIValuesManager = uIValuesManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            MangaCharacter = await _context.MangaCharacter.Where(x => x.Id == id).FirstOrDefaultAsync();

            return Page();
        }
    }
}
