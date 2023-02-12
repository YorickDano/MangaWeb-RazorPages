using MangaWeb.APIClients;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Managers;
using MangaWeb.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ResearchMangaClient _researchMangaClient;

        public IEnumerable<int> CurrentUserFavoritesManga;
        public readonly IStringLocalizer<SharedResource> Localizer;

        public bool IsSeeAll { get; set; } = false;

        public MangaModel(MangaWebContext context,
            UserManager<MangaWebUser> userManager, IAuthorizationService authorizationService,
            ResearchMangaClient researchMangaClient, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
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

            CurrentUserFavoritesManga = (await _userManager.GetUserAsync(User))?.FavoriteManga ?? new List<int>();

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

            var result = await _authorizationService.AuthorizeAsync(User, Manga, "CanManageManga");
            if (!result.Succeeded)
            {
                return RedirectToPage("/Account/AccessDenied",
                    new { area= "Identity", message = "You didn't create this manga, so you cannot delete it", statusCode = 403 });
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
            if(Manga.Language == Language.en)
            {
                Manga.CloneFrom(await _researchMangaClient.UpdateMangaAsync(Manga));
            }
            else
            {
                Manga.CloneFrom(await new ResearchRuMangaClient().UpdateMangaAsync(Manga));
            }
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

        public async Task<IActionResult> OnPostCreateCommentAsync(int? id,string body)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", accessDeniedMessage = "You have no access, you need to log in.", returnUrl = "~/MangaPages/Create" });
            }
            var mangaUser = await _userManager.GetUserAsync(User);
            var manga = await _context.Manga.FirstAsync(x => x.Id == id);
      
            await _context.Comments.AddAsync(new Comment()
            {
                Body = body,
                AuthorName = mangaUser.UserName,
                AuthorImgSrc = mangaUser.ProfileImage,
                Date = DateTime.Now,
                Manga = manga
            });
            await _context.SaveChangesAsync();

            return RedirectToPage("Manga",new {id = id});
        }
    }
}
