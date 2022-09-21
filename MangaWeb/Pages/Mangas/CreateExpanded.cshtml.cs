using MangaWeb.APIClient;
using MangaWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Pages.ExpandedManga
{
    public class CreateExpandedModel : PageModel
    {
        private readonly MangaWebContext _context;

        public CreateExpandedModel(MangaWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required]
        [MinLength(1)]
        public string MangaTitleInput { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            ReseachClient reseachClient = new();
            var Manga = await reseachClient.GetFullManga(MangaTitleInput);
            await _context.FullMangas.AddAsync(Manga);
            await _context.SaveChangesAsync();
            return RedirectToPage($"/Mangas/ExpandedManga", new { id = Manga.Id });
        }
    }
}
