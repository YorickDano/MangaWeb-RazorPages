using MangaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
    public class UsersInfoModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;

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

        public async Task<IActionResult> OnGetBanUser(string id)
        {
            CurrentUser = await _userManager.GetUserAsync(User);

            if(id != CurrentUser.Id)
            {
                await _userManager.SetLockoutEnabledAsync(await _userManager.FindByIdAsync(id), true);
            }

            Users = _userManager.Users.ToList();

            return Page();
        }

        public async Task<IActionResult> OnGetUnbanUser(string id)
        {
            CurrentUser = await _userManager.GetUserAsync(User);

            if (id != CurrentUser.Id)
            {
                await _userManager.SetLockoutEnabledAsync(await _userManager.FindByIdAsync(id), false);
            }

            Users = _userManager.Users.ToList();

            return Page();
        }
    }
}
