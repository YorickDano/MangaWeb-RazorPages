using MangaWeb.APIClient;
using MangaWeb.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MangaWeb.Pages.Manga_s_
{
    public class CreateDefaultModel : PageModel
    {
        private readonly MangaWebContext _context;

        public CreateDefaultModel(MangaWebContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Manga Manga { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {


                if (!ModelState.IsValid || _context.Manga == null || Manga == null)
                {
                    return Page();
                }
                if (_context.Manga.Any(x => x.Title == Manga.Title))
                {
                    TempData["AlertMessage"] = "There are already manga with such title";
                    return Page();
                }
                var genre = string.Empty;
                var genres = Manga.Genre.Split(' ');
                for (int i = 0; i < genres.Length - 1; ++i)
                {
                    if (char.IsUpper(genres[i][0]) && char.IsLower(genres[i + 1][0]))
                    {
                        genre += genres[i];
                        while (char.IsLower(genres[i + 1][0]))
                        {
                            genre += " " + genres[i + 1];
                            ++i;
                            if (i >= genres.Length - 1)
                                break;
                        }
                    }
                    if (!genre.Contains(genres[i]))
                        genre += genres[i] + "\t";
                    else
                        genre += "\t";
                }
                await ApiSetting();
                Manga.Genre = genre.TrimEnd();
                _context.Manga.Add(Manga);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return RedirectToPage("./FreakPage");
            }
            return RedirectToPage("./Index");
        }

        public async Task ApiSetting()
        {
            RestClientApi restClientApi = new RestClientApi();

            Manga.MainImageUrl = await restClientApi.GetMangaProfieImageUrlByTitle(Manga.Title, Enums.SearchType.MangaImage);
            Manga.ReadSiteUrl = await restClientApi.GetUrlOfMangaByTitle(Manga.Title);
        }
    }
}
