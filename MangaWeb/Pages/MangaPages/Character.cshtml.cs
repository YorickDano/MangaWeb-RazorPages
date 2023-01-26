using MangaWeb.Areas.Identity.Data;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages.MangaPages
{
    public class CharacterModel : PageModel
    {
        private readonly MangaWebContext _context;

        public MangaCharacter MangaCharacter;
        public IStringLocalizer<SharedResource> Localizer;

        public CharacterModel(MangaWebContext mangaWebContext, 
            IStringLocalizer<SharedResource> localizer)
        {
            _context = mangaWebContext;
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            MangaCharacter = await _context.MangaCharacter.Where(x => x.Id == id).FirstOrDefaultAsync();

            return Page();
        }
    }
}
