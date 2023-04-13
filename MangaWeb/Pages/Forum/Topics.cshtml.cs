using MangaWeb.Areas.Identity.Data;
using MangaWeb.Filters;
using MangaWeb.Models.ForumModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
    [IsAuthenticatedFilter(PageToReturn = "~/Forum/Topics")]
    public class ForumModel : PageModel
    {
        private readonly MangaWebContext _context;

        public readonly IStringLocalizer<SharedResource> Localizer;
        public readonly UserManager<MangaWebUser> UserManager;
        public MangaWebUser CurrentUser;

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

        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Topics = await _context.Topics.ToListAsync();
            CurrentUser = await UserManager.GetUserAsync(User);
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
            if(string.IsNullOrEmpty(Topic.Title) || string.IsNullOrEmpty(Topic.Description))
            {
                StatusMessage = Localizer["TopicCreationFail"];
                Topics = await _context.Topics.ToListAsync();
                return Page();
            }

            Topic.AuthorId = user.Id;
            _context.Topics.Add(Topic);
            await _context.SaveChangesAsync();

            return RedirectToPage("Topics");
        }
        public async Task<IActionResult> OnPostDeleteTopicAsync(int topicId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", accessDeniedMessage = "You have no access, you need to log in.", returnUrl = "~/MangaPages/Create" });
            }
            var topic = await _context.Topics.FirstOrDefaultAsync(x=>x.Id == topicId);
            var posts = _context.Posts.Where(x => x.TopicID == topicId);
            _context.Posts.RemoveRange(posts);
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return RedirectToPage("Topics");
        }
    }
}
