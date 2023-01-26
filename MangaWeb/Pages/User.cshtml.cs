using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
    public class UserModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly MangaWebContext _context;

        public MangaWebUser User { get; set; }
        public List<Manga> FavoriteManga { get; set; }
        public IStringLocalizer<SharedResource> Localizer;
        public UserModel(UserManager<MangaWebUser> userManager,
            MangaWebContext context, IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _context = context;
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            User = await _userManager.FindByNameAsync(userName);

            if(User == null)
            {
                return NotFound();
            }
            if (User.FavoriteManga != null)
            {
                FavoriteManga = _context.Manga.Where(x => User.FavoriteManga.Contains(x.Id)).ToList();
            }

            return Page();
        }
    }
}
