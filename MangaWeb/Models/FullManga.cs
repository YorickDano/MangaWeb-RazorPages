using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Models
{
    public class FullManga
    {
        public int Id { get; set; }
        [Display(Name = "Title")]
        public string OriginTitle { get; set; } = "Title";
        [Display(Name = "Image")]
        public string MangaImageUrl { get; set; } = "https://t7.nhentai.net/galleries/1510625/cover.png";
        public string Description { get; set; } = "Description";
        public string Published { get; set; } = "Published";
        [Display(Name = "Volumes")]
        public int CountOfVolume { get; set; }
        [Display(Name = "Chapters")]
        public int CountOfChapters { get; set; }
        public double Score { get; set; }
        public int Ranked { get; set; }
        public int Popularity { get; set; }
        public List<string> Geners { get; set; } = new List<string> { "Gener" };
        public MangaStatus Status { get; set; }
        public List<string> Autors { get; set; }
        public List<MangaCharacter> Characters { get; set; } = new List<MangaCharacter>();
        public int YearOfIssue { get; set; }
    }
    public enum MangaStatus
    {
        Publishing,
        Finished,
        Announced,
        Frozen
    }
}
