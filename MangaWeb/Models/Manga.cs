using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Models
{
    public class Manga
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = "Title";
        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string Genre { get; set; } = "Genre";
        [Required]
        public string Description { get; set; } = "Description";
        [Display(Name = "Read")]
        public string ReadSiteUrl { get; set; } = "https://mangalib.me/";
        [Display(Name = "Image")]
        public string MainImageUrl { get; set; } = "https://t7.nhentai.net/galleries/1510625/cover.png";
        public string[] AnimeImagesUrls { get; set; } = Array.Empty<string>();
        public string[] HentaiImagesUrls { get; set; } = Array.Empty<string>();
    }
}
