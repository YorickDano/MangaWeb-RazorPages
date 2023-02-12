using MangaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Pages
{
    [Authorize]
    public class MessageModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;
        private readonly MangaWebContext _context;

        public readonly IStringLocalizer<SharedResource> Localizer;

        public MessageModel(IStringLocalizer<SharedResource> localizer,
            UserManager<MangaWebUser> userManager,
            MangaWebContext context)
        {
            Localizer = localizer;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(string userNameWriteTo)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var userWriteTo = await _userManager.FindByNameAsync(userNameWriteTo);



            return Page();
        }
        public async Task<IActionResult> OnPostSendMessageAsync(string messageInput)
        {


            return Page();
        }
    }
}
