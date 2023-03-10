using MangaWeb.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace MangaWeb.Extensions
{
    public static class MangaWebUserExtension
    {
        public static bool IsGotNewMessages(this MangaWebUser user, MangaWebContext context)
        {
            return context.Messages.Where(x => (x.UserNameTo == user.UserName)
                   && !x.IsViewed)?.Count() > 0;
        }
    }
}
