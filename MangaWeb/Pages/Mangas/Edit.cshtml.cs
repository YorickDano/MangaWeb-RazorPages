using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MangaWeb.Data;
using MangaWeb.Models;

namespace MangaWeb.Pages.Manga_s_
{
    public class EditModel : PageModel
    {
        private readonly MangaWebContext _context;

        public EditModel(MangaWebContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Manga Manga { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Manga == null)
            {
                return NotFound();
            }

            var manga =  await _context.Manga.FirstOrDefaultAsync(m => m.Id == id);
            if (manga == null)
            {
                return NotFound();
            }
            Manga = manga;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Manga).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MangaExists(Manga.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool MangaExists(int id)
        {
          return (_context.Manga?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
