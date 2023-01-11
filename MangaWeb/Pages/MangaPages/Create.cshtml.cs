using MangaWeb.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MangaWeb.Pages.MangaPages
{
    public class CreateModel : PageModel
    {
        public UIValuesManager UIValuesManager;

        public CreateModel(UIValuesManager uIValuesManager)
        {
            UIValuesManager = uIValuesManager;
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
