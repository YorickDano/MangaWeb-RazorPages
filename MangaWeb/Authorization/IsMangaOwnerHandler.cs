using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MangaWeb.Authorization
{
    public class IsMangaOwnerHandler : AuthorizationHandler<IsMangaOwnerRequirement, Manga>
    {
        private readonly UserManager<MangaWebUser> _userManager;
        public IsMangaOwnerHandler(UserManager<MangaWebUser> userManager)
        {
            _userManager = userManager;
        }
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, IsMangaOwnerRequirement requirement, Manga manga)
        {
            var appUser = await _userManager.GetUserAsync(context.User);

            if (appUser == null || !appUser.CreatedManga?.Contains(manga.Id) != null 
                || appUser.Role == Role.User)
            {
                return;
            }

            context.Succeed(requirement);
        }
    }
}
