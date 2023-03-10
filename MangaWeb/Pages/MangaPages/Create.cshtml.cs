using MangaWeb.Filters;
using MangaWeb.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages.MangaPages
{
    [IsAuthenticatedFilter(PageToReturn = "~/MangaPages/Create")]
    public class CreateModel : PageModel
    {
        public IStringLocalizer<SharedResource> Localizer;
  
        public CreateModel(IStringLocalizer<SharedResource> localizer)
        {
            Localizer = localizer;
        }
        
        public async Task<IActionResult> OnGetAsync()
        {

            return Page();
        }
    }
}
