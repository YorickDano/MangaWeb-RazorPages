using MangaWeb.APIClient;
using MangaWeb.APIClients;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MangaWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGetTest()
        {
            var client = new MangaCharacterClient();
           await client.GetAllCharacters("Kaguya-sama wa Kokurasetai - Tensai-tachi no Renai Zunousen", FullManga.Empty);
            return Page();
        }

        public void OnGet()
        {

        }
    }
}