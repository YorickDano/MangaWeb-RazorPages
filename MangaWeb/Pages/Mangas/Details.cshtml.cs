using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MangaWeb.Data;
using MangaWeb.Models;
using MangaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using MangaWeb.APIClient;
using MangaWeb.DataBaseHandler;

namespace MangaWeb.Pages.Manga_s_
{
    public class DetailsModel : PageModel
    {
        private readonly MangaWebContext _context;
        private readonly UserManager<MangaWebUser> _userManager;
        public List<string> AnimeImagesUrls { get; set; }
        public List<string> HentaiImagesUrls { get; set; }
        public MangaWebUser? MangaWebUser { get; set; } = default!;

        public DetailsModel(MangaWebContext context, UserManager<MangaWebUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Manga? Manga { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            MangaWebUser = await _userManager.GetUserAsync(User);

            if (id is null || _context.Manga is null || MangaWebUser is null)
            {
                return NotFound();
            }

            var manga = await _context.Manga.FirstOrDefaultAsync(m => m.Id == id);
            if (manga is null)
            {
                return NotFound();
            }
            if (manga.AnimeImagesUrls == null || manga.AnimeImagesUrls.Length == 0)
            {
                await manga.UpdateMangaImagesUrls();
                await _context.SaveChangesAsync();
            }

            Manga = manga;

            AnimeImagesUrls = Manga.AnimeImagesUrls.ToList();
            HentaiImagesUrls = Manga.HentaiImagesUrls.ToList();
            return Page();
        }
    }
}
