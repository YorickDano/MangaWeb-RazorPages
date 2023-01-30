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
    public class CharacterModel : PageModel
    {
        private readonly MangaWebContext _context;
        private readonly UserManager<MangaWebUser> _userManager;

        public MangaCharacter MangaCharacter;
        public IStringLocalizer<SharedResource> Localizer;
        public IList<Comment> Comments { get; private set; }

        public CharacterModel(MangaWebContext mangaWebContext, 
            IStringLocalizer<SharedResource> localizer,
            UserManager<MangaWebUser> userManager)
        {
            _context = mangaWebContext;
            Localizer = localizer;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            MangaCharacter = await _context.MangaCharacter.Where(x => x.Id == id).FirstOrDefaultAsync();
            Comments = await _context.Comments.Where(x => x.Character.Id == id).ToListAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostCreateCommentAsync(int? id, string body)
        {
            var mangaUser = await _userManager.GetUserAsync(User);
            var character = await _context.MangaCharacter.FirstAsync(x => x.Id == id);
            var comment = new Comment()
            {
                Body = body,
                AuthorName = mangaUser.UserName,
                AuthorImgSrc = mangaUser.ProfileImage,
                Date = DateTime.Now,
                Character = character
            };
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return RedirectToPage("Character", new { id });
        }
    }
}
