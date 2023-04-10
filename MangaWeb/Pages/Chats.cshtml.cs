using MangaWeb.Areas.Identity.Data;
using MangaWeb.Filters;
using MangaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Permissions;

namespace MangaWeb.Pages
{
    [IsAuthenticatedFilter]
    public class ChatsModel : PageModel
    {
        private readonly MangaWebContext _context;
        private readonly UserManager<MangaWebUser> _userManager;
        
        public List<Conversation> Conversations { get; set; }
        public readonly IStringLocalizer<SharedResource> Localizer;
        public string CurrentUserName;
        public string CurrentUserImageSrc;
        public ChatsModel(MangaWebContext context, 
            UserManager<MangaWebUser> userManager,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _userManager = userManager;
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
           
            CurrentUserImageSrc = currentUser.ProfileImage;
            CurrentUserName = currentUser.UserName;
            Conversations = _context.Conversations.Include(x=>x.FirstUser).Include(x=>x.SecondUser)
                .Where(x => x.FirstUser.UserName == currentUser.UserName 
                || x.SecondUser.UserName == currentUser.UserName).Distinct().ToList();

            return Page();
        }
    }
}
