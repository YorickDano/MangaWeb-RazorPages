using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MangaWeb.Pages.Mangas
{
    public class FreakPageModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool Enable { get; set; }             
        }

        public void OnGet()
        {
        }
    }
}
