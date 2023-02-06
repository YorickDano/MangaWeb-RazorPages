using MangaWeb.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages.MangaPages
{
    public class CreateModel : PageModel
    {
        public IStringLocalizer<SharedResource> Localizer;
  
        public CreateModel(IStringLocalizer<SharedResource> localizer)
        {
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                 return RedirectToPage("/Account/Login", new { area = "Identity", accessDeniedMessage = "You have no access, you need to log in.", returnUrl = "~/MangaPages/Create" });
            }

            return Page();
        }
    }
}
