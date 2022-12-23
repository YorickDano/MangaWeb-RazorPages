using MangaWeb.APIClients;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Pages.MangaPages
{
    public class CreateExpandedModel : PageModel
    {
        private readonly MangaWebContext _context;
        private readonly UserManager<MangaWebUser> _userManager;

        public CreateExpandedModel(MangaWebContext context, UserManager<MangaWebUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        [Required]
        [MinLength(1)]
        public string MangaTitleInput { get; set; }
        private readonly string ClientID = "_si76ZjflE4TgeVKK-1poihsD2HU6SV9Xgy3RSV2ZMg";
        private readonly string ClientSecret = "ZfzQAjn0E6zHe8JB0aAbJ8W-hMnStDAB_EDH8XD8o7I";
        [BindProperty]
        public bool IsRussian { get; set; }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return new ForbidResult();
            }
            var manga = new Manga();

            if (!IsRussian)
            {
                ResearchMangaClient reseachClient = new();
                 manga = await reseachClient.GetFullManga(MangaTitleInput);
            }
            else
            {
               
                ResearchRuMangaClient reseachClient = new();
                manga = await reseachClient.GetManga(MangaTitleInput);
            }

            await _context.Manga.AddAsync(manga);

            await _context.SaveChangesAsync();
            if (appUser.CreatedManga == null)
            {
                appUser.CreatedManga = new List<int>() { manga.Id };
            }
            else
            {
                appUser.CreatedManga.Add(manga.Id);
            }
            await _userManager.UpdateAsync(appUser);
            GC.Collect();
            return RedirectToPage($"/MangaPages/Manga", new { id = manga.Id });
        }
    }
}
