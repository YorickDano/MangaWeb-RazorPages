using MangaWeb.APIClients;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MangaWeb.Pages.MangaPages
{
    public class MangaModel : PageModel
    {
        private readonly MangaWebContext _context;
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        public IEnumerable<int> CurrentUserFavoritesManga;
        public bool IsSeeAll { get; set; } = false;

        public MangaModel(MangaWebContext context, 
            UserManager<MangaWebUser> userManager, IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public Manga? Manga { get; set; }


        public async Task<IActionResult> OnGet(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            CurrentUserFavoritesManga = (await _userManager.GetUserAsync(User))?.FavoriteManga ?? new List<int>();

            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            
            if (Manga is null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetMangaDeletionAsync(int? id)
        {
            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            var result = await _authorizationService.AuthorizeAsync(User, Manga, "CanManageManga");
            if (!result.Succeeded)
            {
                return new ForbidResult();
            }

            var charactersOfMaga = _context.MangaCharacter.Where(x => x.Manga.Id == id);
            foreach (var character in charactersOfMaga)
            {
                _context.MangaCharacter.Remove(character);
            }
            _context.Manga.Remove(Manga);
            await _context.SaveChangesAsync();

            return Redirect("../Index");
        }


        public async Task<IActionResult> OnGetDownloadDataAsync(int? id)
        {
            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id) ?? Manga.CreateNew();

            return File(JsonManager.SerializeToByteArray(Manga), "application/json", "MangaData.json");
        }

        public async Task<IActionResult> OnGetAddToFavoriteAsync(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            CurrentUserFavoritesManga = (await _userManager.GetUserAsync(User))?.FavoriteManga ?? new List<int>();

            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            if (Manga is null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            user.FavoriteManga ??= new List<int>();

            if (user.FavoriteManga.Contains(Manga.Id))
            {
                user.FavoriteManga.Remove(Manga.Id);
            }
            else
            {
                user.FavoriteManga.Add(Manga.Id);
            }

            await _userManager.UpdateAsync(user);

            return Redirect($"/MangaPages/ExpandedManga?id={id}");
        }

        public async Task<IActionResult> OnGetUpdateMangaAsync(int id)
        {
            CurrentUserFavoritesManga = (await _userManager.GetUserAsync(User))?.FavoriteManga ?? new List<int>();

            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            if (Manga is null)
            {
                return NotFound();
            }

            Manga.CloneFrom(await new ResearchMangaClient().UpdateMangaAsync(Manga));
            _context.Manga.Update(Manga);
            await _context.SaveChangesAsync();

            return Redirect($"/MangaPages/ExpandedManga?id={id}");
        }
        public async Task<IActionResult> OnGetEditMangaAsync(int id)
        {
            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            if (Manga is null)
            {
                return NotFound();
            }

            return Redirect($"/MangaPages/Edit?id={id}");
        }
        
    }
}
