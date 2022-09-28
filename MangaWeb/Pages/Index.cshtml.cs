using MangaWeb.APIClient;
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
            var client = new MangaReadClient();
            var a = await client.GetPageByTitleVolumeChapter("Kaguya-sama wa Kokurasetai - Tensai-tachi no Renai Zunousen");
            return Page();
        }

        public void OnGet()
        {

        }
    }
}