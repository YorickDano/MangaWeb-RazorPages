using MangaWeb.Areas.Identity.Data;
using MangaWeb.Filters;
using MangaWeb.Models;
using MangaWeb.Models.ForumModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages.Forum
{
    
    public class PostsModel : PageModel
    {
        private readonly MangaWebContext _context;
        public readonly UserManager<MangaWebUser> _userManager;

        public readonly IStringLocalizer<SharedResource> Localizer;
        public IList<Post> Posts { get; private set; }
        public Topic Topic { get; private set; }
        public string StatusMessage { get; set; }

        public PostsModel(IStringLocalizer<SharedResource> localizer,
            MangaWebContext context,
            UserManager<MangaWebUser> userManager)
        {
            Localizer = localizer;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Topic = await _context.Topics.FindAsync(id);
            if(Topic == null)
            {
                return NotFound();
            } 
            Posts = await _context.Posts.Where(x => x.TopicID == id).ToListAsync();

            return Page();
        }
        public async Task<IActionResult> OnPostCreatePostAsync(int? id, string body)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity", accessDeniedMessage = Localizer["NoAccess"], returnUrl = "~/MangaPages/Create" });
            }
            Topic = await _context.Topics.FindAsync(id);
            if (Topic == null)
            {
                return NotFound();
            }
            var mangaUser = await _userManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(body))
            {
                Topic = await _context.Topics.FindAsync(id);
                if (Topic == null)
                {
                    return NotFound();
                }
                Posts = await _context.Posts.Where(x => x.TopicID == id).ToListAsync();
                StatusMessage = Localizer["PostCreationFail"];
                return Page();
            }

            var post = new Post()
            {
                AuthorId = mangaUser.Id,
                TopicID = (int)id,
                Content = body,
                Date = DateTime.Now
            };
            if(Topic.Posts == null) Topic.Posts = new List<Post>();

            await _context.Posts.AddAsync(post);
            Topic.Posts.Add(post);
            _context.Topics.Update(Topic);
            await _context.SaveChangesAsync();

            return RedirectToPage("Posts", new { id });
        }
    }
}
