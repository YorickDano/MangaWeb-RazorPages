using MangaWeb.Areas.Identity.Data;
using MangaWeb.Filters;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
    [IsAuthenticatedFilter]
    public class UsersInfoModel : PageModel
    {
        public readonly UserManager<MangaWebUser> _userManager;

        public MangaWebUser CurrentUser;

        public List<MangaWebUser>? Users { get; set; }
        public IStringLocalizer<SharedResource> Localizer;
        public UsersInfoModel(UserManager<MangaWebUser> userManager,
            IStringLocalizer<SharedResource> localizer)
        {
            _userManager = userManager;
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUser = await _userManager.GetUserAsync(User);

            Users = _userManager.Users.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostBanUserAsync(string id)
        {
            CurrentUser = await _userManager.GetUserAsync(User);

            if (id != CurrentUser.Id)
            {
                var user = await _userManager.FindByIdAsync(id);
                var result = await _userManager.SetLockoutEnabledAsync(user, true);
                var resultdate = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(30));

                if (resultdate.Succeeded)
                {
                    await _userManager.UpdateAsync(user);
                }
            }

            Users = _userManager.Users.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostUnbanUserAsync(string id)
        {
            CurrentUser = await _userManager.GetUserAsync(User);

            if (id != CurrentUser.Id)
            {
                var user = await _userManager.FindByIdAsync(id);
                var result = await _userManager.SetLockoutEnabledAsync(user, false);
                if (result.Succeeded)
                {
                    await _userManager.UpdateAsync(user);
                }
            }

            Users = _userManager.Users.ToList();

            return Page();
        }
    }
}
