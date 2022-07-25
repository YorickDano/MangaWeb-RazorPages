using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MangaWeb.Data;
using MangaWeb.Models;

namespace MangaWeb.Pages.Manga_s_
{
    public class DetailsModel : PageModel
    {
        private readonly MangaWebContext _context;

        public DetailsModel(MangaWebContext context)
        {
            _context = context;
        }

      public Manga Manga { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Manga == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga.FirstOrDefaultAsync(m => m.Id == id);
            if (manga == null)
            {
                return NotFound();
            }
            else 
            {
                Manga = manga;
            }
            return Page();
        }
    }
}
