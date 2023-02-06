using MangaWeb.APIClients;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Pages.MangaPages
{
    public class CreateExpandedModel : PageModel
    {
        private readonly MangaWebContext _context;
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly ResearchMangaClient _researchMangaClient;
        private readonly ResearchRuMangaClient _researchRuMangaClient;

        public readonly IStringLocalizer<SharedResource> Localizer;
        public string ServerUnavailableMessage { get; private set; } = null;
        public CreateExpandedModel(MangaWebContext context, 
            UserManager<MangaWebUser> userManager,ResearchMangaClient researchMangaClient, 
            ResearchRuMangaClient researchRuMangaClient, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _userManager = userManager;
            _researchMangaClient = researchMangaClient;
            _researchRuMangaClient = researchRuMangaClient;
            Localizer = localizer;
        }

        [BindProperty]
        [Required]
        [MinLength(1)]
        public string MangaTitleInput { get; set; }

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
                manga = await _researchMangaClient.GetFullManga(MangaTitleInput);
            }
            else
            {
                manga = await _researchRuMangaClient.GetManga(MangaTitleInput);
            }

            if(manga == null)
            {
                ServerUnavailableMessage = "Server is not available, please try a few minute later.";
                return Page();
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

            return RedirectToPage($"/MangaPages/Manga", new { id = manga.Id });
        }
    }
}
