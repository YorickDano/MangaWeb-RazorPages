using MangaWeb.APIClients;
using MangaWeb.Authorization;
using MangaWeb.Filters;
using Microsoft.AspNetCore.Authorization;

namespace MangaWeb.Managers
{
    public static class WebApplicationBuilderServicesExtension
    {
        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<AnimeImageClient>();
            builder.Services.AddScoped<IAuthorizationHandler, IsMangaOwnerHandler>();
            builder.Services.AddScoped<MangaCharacterClient>();
            builder.Services.AddScoped<ResearchMangaClient>();
            builder.Services.AddScoped<ResearchRuMangaClient>();
        }
    }
}
