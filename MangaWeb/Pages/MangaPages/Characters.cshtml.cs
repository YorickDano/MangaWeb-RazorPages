using MangaWeb;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Composition;

namespace MangaWebNot.Pages.MangaPages
{
    public class CharactersModel : PageModel
    {
        private readonly MangaWebContext _context;

        public IStringLocalizer<SharedResource> Localizer { get; set; }
        public List<MangaCharacter> Characters { get; set; }

        public CharactersModel(MangaWebContext context,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            Localizer = localizer;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var distinctedCharacters = _context.MangaCharacter.AsEnumerable().DistinctBy(x => x.Name);
            Characters = distinctedCharacters.ToList();
            return Page();
        }
    }
}
