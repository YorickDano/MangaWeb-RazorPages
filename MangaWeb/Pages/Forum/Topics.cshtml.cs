using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models.ForumModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
    public class ForumModel : PageModel
    {
        private readonly MangaWebContext _context;

        public readonly IStringLocalizer<SharedResource> Localizer;
        public readonly UserManager<MangaWebUser> UserManager;

        public ForumModel(IStringLocalizer<SharedResource> localizer,
            MangaWebContext context, UserManager<MangaWebUser> userManager)
        {
            Localizer = localizer;
            _context = context;
            UserManager = userManager;
        }

        [BindProperty]
        public Topic Topic { get; set; }


        public List<Topic> Topics { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Topics = await _context.Topics.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCreateTopicAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", accessDeniedMessage = "You have no access, you need to log in.", returnUrl = "~/MangaPages/Create" });
            }
            var user = await UserManager.GetUserAsync(User);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Topic.AuthorName = User.Identity.Name;
            Topic.AuthorImgSrc = user.ProfileImage;
            _context.Topics.Add(Topic);
            await _context.SaveChangesAsync();

            return RedirectToPage("Topics");
        }
    
    }
}
