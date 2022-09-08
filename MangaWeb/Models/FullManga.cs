using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Models
{
    public class FullManga
    {
        public int Id { get; set; }
        [Display(Name ="Title")]
        public string OriginTitle { get; set; }
        [Display(Name = "Image")]
        public string MangaImageUrl { get; set; }
        public string Description { get; set; }
        public string Published { get; set; }
        [Display(Name = "Volumes")]
        public int? CountOfVolume { get; set; }
        [Display(Name = "Chapters")]
        public int? CountOfChapters { get; set; }
        public double Score { get; set; }
        public int Ranked { get; set; }
        public int Popularity { get; set; }
        public List<string> Geners { get; set; }
        public MangaStatus Status { get; set; }
        public List<string> Autors { get; set; }
        public List<MangaCharacter>? Characters { get; set; }
    }
    public enum MangaStatus
    {
        Publishing,
        Finished,
        Announced,
        Frozen
    }
}
