using MangaWeb.Models;
using Microsoft.AspNetCore.Localization;

namespace MangaWeb.Managers
{
    public static class LanguageManager
    {
        public static bool IsEnglish;
        static LanguageManager()
        {
            IsEnglish = true;
        }

        public static void Set(this HttpResponse response, string language)
        {
            response.Cookies.Append(
             CookieRequestCultureProvider.DefaultCookieName,
             CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
             new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            IsEnglish = language == "en";
        } 

        public static bool IsEnlish() => IsEnglish;
    }
}
