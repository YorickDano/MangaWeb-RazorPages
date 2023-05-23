using MangaWeb.APIClients;
using MangaWeb.Areas.Identity.Data;
using MangaWeb.Filters;
using MangaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MangaWeb.Pages.MangaPages
{
    [ModelStateFilter]
    public class CreateDefaultModel : PageModel
    {
        private readonly MangaWebContext _context;
        private readonly UserManager<MangaWebUser> _userManager;
        public readonly IStringLocalizer<SharedResource> Localizer;


        public CreateDefaultModel(MangaWebContext context,
            IStringLocalizer<SharedResource> localizer,
            UserManager<MangaWebUser> userManager)
        {
            _context = context;
            Localizer = localizer;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Manga Manga { get; set; } = default!;

        [BindProperty]
        public InputModel Input { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            var manga = new Manga();
            try
            {
                if (ModelState.IsValid)
                {
                    manga.OriginTitle = Input.OriginTitle;
                    manga.Popularity = Input.Popularity;
                    manga.Authors = Input.Authors.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    manga.Status = Input.Status;
                    manga.CountOfChapters = Input.CountOfChapters;
                    manga.CountOfVolume = Input.CountOfVolume;
                    manga.Published = Input.Published;
                    manga.Ranked = Input.Ranked;
                    manga.Score = Input.Score;
                    manga.Description = Input.Description;
                    manga.YearOfIssue = Input.YearOfIssue;
                    manga.Genres = Input.Genres.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    using (var ms = new MemoryStream())
                    {
                        Input.ImageFile.CopyTo(ms);
                        var imgSrc = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(ms.ToArray()));
                        manga.MangaImageUrl = imgSrc;
                    }
                    manga.Creator = (await _userManager.GetUserAsync(User)).UserName;
                    await _context.Manga.AddAsync(manga);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                return RedirectToPage("./Error");
            }

            return RedirectToPage("./Manga", new { id = manga.Id });
        }

        public void ApiSetting()
        {
            RestClientApi restClientApi = new RestClientApi();

            //  Manga.MainImageUrl = await restClientApi.GetMangaProfieImageUrlByTitle(Manga.Title, Enums.SearchType.MangaImage);
            //  Manga.ReadSiteUrl = await restClientApi.GetUrlOfMangaByTitle(Manga.Title);
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
            public float Score { get; set; }
            public int Ranked { get; set; }
            public int Popularity { get; set; }
            public string Genres { get; set; }
            public MangaStatus Status { get; set; }
            public string Authors { get; set; }
            public List<MangaCharacter> Characters { get; set; } = new List<MangaCharacter>();
            [Display(Name = "Year of issue")]
            public int YearOfIssue { get; set; }

            [DataType(DataType.Upload)]
            public IFormFile ImageFile { get; set; }
        }
    }
}
