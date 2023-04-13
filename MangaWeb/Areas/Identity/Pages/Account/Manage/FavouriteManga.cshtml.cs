using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Areas.Identity.Pages.Account.Manage
{
    public class FavouriteMangaModel : PageModel
    {
        private readonly MangaWebContext _context;
        private readonly UserManager<MangaWebUser> _userManager;
        public readonly IStringLocalizer<SharedResource> Localizer;

        public IEnumerable<Manga> FavouriteManga;

        public FavouriteMangaModel(MangaWebContext context, UserManager<MangaWebUser> userManager,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _userManager = userManager;
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if(user == null || user.FavoriteManga == null)
            {
                return Page();
            }
            FavouriteManga = _context.Manga.Where(x => user.FavoriteManga.Contains(x.Id)).ToArray();
            
            return Page();
        }
    }
}
