using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangaWeb.Models
{
    public class Manga
    {     
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string? Genre { get; set; }
        [Required]
        public string? Description { get; set; }
        [Display(Name = "Read")]
        public string? ReadSiteUrl { get; set; }
        [Display(Name ="Image")]
        public string? MainImageUrl { get; set; }
        public string[]? AnimeImagesUrls { get; set; }
        public string[]? HentaiImagesUrls { get; set; }
    }
}
