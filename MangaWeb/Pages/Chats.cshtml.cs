using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
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
            Conversations = _context.Conversations.Where(x => x.FirstUserName == currentUser.UserName
            || x.SecondUserName == currentUser.UserName).ToList();

            return Page();
        }
    }
}
