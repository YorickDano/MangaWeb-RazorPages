using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.Diagnostics;

namespace MangaWeb.Pages
{

    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;
        public IStringLocalizer<SharedResource> Localizer;
        public ErrorModel(ILogger<ErrorModel> logger, 
            IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            Localizer = localizer;
        }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}