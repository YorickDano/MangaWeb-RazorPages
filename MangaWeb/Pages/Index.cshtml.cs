using AspNetCore.RouteAnalyzer;
using MangaWeb.APIClients;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public readonly IStringLocalizer<SharedResource> Localizer;

        public IndexModel(ILogger<IndexModel> logger, 
            IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            Localizer = localizer; 
        }

        public async Task<IActionResult> OnGetTest()
        {
            var client = new MangaCharacterClient();
          // await client.GetAllCharacters("Kaguya-sama wa Kokurasetai - Tensai-tachi no Renai Zunousen", Manga.Empty);
            return Page();
        }

        public async void OnGetAsync()
        {

        }
    }
}