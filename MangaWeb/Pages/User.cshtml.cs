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

        public MangaWebUser MangaWebUser { get; set; }
        public MangaWebUser CurrentUser;
        public List<Manga> FavoriteManga { get; set; }
        public IStringLocalizer<SharedResource> Localizer;
        public string YouWriteToYourselfMessage { get; set; }
        public UserModel(UserManager<MangaWebUser> userManager,
            MangaWebContext context, IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            _context = context;
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            MangaWebUser = await _userManager.FindByNameAsync(userName);
            CurrentUser = await _userManager.GetUserAsync(User);
            if (MangaWebUser == null)
            {
                return NotFound();
            }
            if (MangaWebUser.FavoriteManga != null)
            {
                FavoriteManga = _context.Manga.Where(x => MangaWebUser.FavoriteManga.Contains(x.Id)).ToList();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostBlockUserAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            
            if (user == null)
            {
                var result = await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(10)));
            }

            return RedirectToPage("../User", new {userName});
        }

        public async Task<IActionResult> OnPostWriteToUserAsync(string userName)
        {
            MangaWebUser = await _userManager.FindByNameAsync(userName);
            CurrentUser = await _userManager.GetUserAsync(User);
            if(CurrentUser.UserName == userName)
            {
                YouWriteToYourselfMessage = "You can't write to yourself.";
                if (MangaWebUser.FavoriteManga != null)
                {
                    FavoriteManga = _context.Manga.Where(x => MangaWebUser.FavoriteManga.Contains(x.Id)).ToList();
                }
                return Page();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return Page();
            }

            return RedirectToPage("Conversation", new
            {
                userNameWriteTo = userName
            });
        }
    }
}
