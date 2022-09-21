using MangaWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MangaWeb.Pages.Shared
{
    public class UsersInfoModel : PageModel
    {
        private readonly UserManager<MangaWebUser> _userManager;

        public List<MangaWebUser>? Users { get; set; }

        public UsersInfoModel(UserManager<MangaWebUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            Users = _userManager.Users.ToList();
        }
    }
}
