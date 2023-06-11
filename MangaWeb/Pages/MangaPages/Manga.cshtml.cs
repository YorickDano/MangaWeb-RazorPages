using MangaWeb.APIClients;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


namespace MangaWeb.Pages.MangaPages
{
    public class MangaModel : PageModel
    {
        private readonly MangaWebContext _context;
        public readonly UserManager<MangaWebUser> _userManager;
        private readonly ResearchMangaClient _researchMangaClient;

        public IEnumerable<int> CurrentUserFavoritesManga;
        public readonly IStringLocalizer<SharedResource> Localizer;

        public MangaWebUser CurrentUser;

        public bool IsSeeAll { get; set; } = false;

        public MangaModel(MangaWebContext context,
            UserManager<MangaWebUser> userManager, ResearchMangaClient researchMangaClient,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _userManager = userManager;
            Localizer = localizer;
            _researchMangaClient = researchMangaClient;
        }

        public Manga? Manga { get; set; }
        public IList<Comment> Comments { get; private set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }
            CurrentUser = await _userManager.GetUserAsync(User);
            CurrentUserFavoritesManga = CurrentUser?.FavoriteManga ?? new List<int>();

            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            
            if (Manga is null)
            {
                return NotFound();
            }

            Comments = await _context.Comments.Where(x => x.Manga.Id == id).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnGetMangaDeletionAsync(int? id)
        {
            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            if ((await _userManager.GetUserAsync(User)).Role == Role.User)
            {
                return RedirectToPage("/Account/AccessDenied",
                    new { area = "Identity", message = "You didn't create this manga, so you cannot delete it", statusCode = 403 });
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
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", accessDeniedMessage = Localizer["NoAccess"], returnUrl = "~/MangaPages/Create" });
            }
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

            return Redirect($"/MangaPages/Manga?id={id}");
        }

        public async Task<IActionResult> OnGetUpdateMangaAsync(int id)
        {
            CurrentUserFavoritesManga = (await _userManager.GetUserAsync(User))?.FavoriteManga ?? new List<int>();

            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(y => y.Id == id);

            if (Manga is null)
            {
                return NotFound();
            }

            Manga.CloneFrom(Manga.Language == Language.en
                ? await _researchMangaClient.UpdateMangaAsync(Manga, Enumerable.Empty<string>())
                : await new ResearchRuMangaClient().UpdateMangaAsync(Manga, Enumerable.Empty<string>()));

            _context.Manga.Update(Manga);
            await _context.SaveChangesAsync();

            return Redirect($"/MangaPages/Manga?id={id}");
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

        public async Task<IActionResult> OnPostCreateCommentAsync(int? id, string body)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", accessDeniedMessage = Localizer["NoAccess"], returnUrl = "~/MangaPages" });
            }
            var mangaUser = await _userManager.GetUserAsync(User);
            var manga = await _context.Manga.FirstAsync(x => x.Id == id);

            await _context.Comments.AddAsync(new Comment()
            {
                Body = body,
                AuthorId = mangaUser.Id,
                Date = DateTime.Now,
                Manga = manga
            });
            await _context.SaveChangesAsync();

            return RedirectToPage("Manga", new { id });
        }
    }
}
