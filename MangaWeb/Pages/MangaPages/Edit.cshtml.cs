using MangaWeb.Areas.Identity.Data;
using MangaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Pages.MangaPages
{
    public class EditModel : PageModel
    {
        private readonly MangaWebContext _context;
        public Manga? Manga { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }
        public EditModel(MangaWebContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(x => x.Id == id);

            if (Manga == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Manga = await _context.Manga.Include(x => x.Characters).FirstOrDefaultAsync(x => x.Id == id);
            var score = double.Parse(Input.Score.Replace(".", ",").Trim());
            Manga.OriginTitle = Input.OriginTitle;
            Manga.Description = Input.Description;
            Manga.Autors = new List<string>(Input.Autors.Split(",", StringSplitOptions.RemoveEmptyEntries));
            Manga.Score = score;
            Manga.CountOfChapters = Input.CountOfChapters;
            Manga.CountOfVolume = Input.CountOfVolume;
            Manga.Status = Input.Status;
            Manga.Genres = new List<string>(Input.Genres.Split(",", StringSplitOptions.RemoveEmptyEntries));
            if (Input.ImageFile != null)
            {
                using (var ms = new MemoryStream())
                {
                    Input.ImageFile.CopyTo(ms);
                    var imgSrc = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(ms.ToArray()));
                    Manga.MangaImageUrl = imgSrc;
                }
            }
            if (Input.CharactersRemove != null)
            {
                Manga.Characters = new List<MangaCharacter>(Manga.Characters.Where(x => !Input.CharactersRemove.Contains(x.Id)));

            }
            Manga.Popularity = Input.Popularity;
            Manga.Published = Input.Published;
            Manga.Ranked = Input.Ranked;
            Manga.YearOfIssue = Input.YearOfIssue;

            _context.Manga.Update(Manga);
            await _context.SaveChangesAsync();
            return Redirect($"/MangaPages/Manga?id={id}");
        }

        public class InputModel
        {
            public string OriginTitle { get; set; } = "Title";
            [Display(Name = "Manga image")]
            public string MangaImageUrl { get; set; } = "https://t7.nhentai.net/galleries/1510625/cover.png";
            public string Description { get; set; } = "Description";
            public string Published { get; set; } = "Published";
            [Display(Name = "Volumes")]
            public int CountOfVolume { get; set; }
            [Display(Name = "Chapters")]
            public int CountOfChapters { get; set; }
            public string Score { get; set; }
            public double ScoreDec { get; set; }
            public int Ranked { get; set; }
            public int Popularity { get; set; }
            public string Genres { get; set; }
            public MangaStatus Status { get; set; }
            public string Autors { get; set; }
            public List<MangaCharacter> Characters { get; set; } = new List<MangaCharacter>();
            [Display(Name = "Year of issue")]
            public int YearOfIssue { get; set; }

            [DataType(DataType.Upload)]

            public IFormFile ImageFile { get; set; }
            public int[] CharactersRemove { get; set; }
        }

    }
}
