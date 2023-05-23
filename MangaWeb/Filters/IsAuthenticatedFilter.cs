using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;

namespace MangaWeb.Filters
{
    public class IsAuthenticatedFilter : Attribute, IAsyncPageFilter
    {
        public string PageToReturn { get; set; }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context is null) throw new ArgumentNullException("The context was null");
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToPageResult("/Account/Login", new { area = "Identity", accessDeniedMessage = "You have no access, you need to log in.", returnUrl = PageToReturn });
            }
            else
            {
                await next();
            }
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await Task.CompletedTask;
        }
    }
}
