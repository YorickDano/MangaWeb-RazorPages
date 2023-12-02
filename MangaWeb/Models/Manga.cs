using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Models
{
    public class Manga
    {
        public int Id { get; set; }
        [Display(Name = "Title")]
        public string OriginTitle { get; set; }
        [Display(Name = "ImageStr")]
        public string MangaImageUrl { get; set; } = "https://t7.nhentai.net/galleries/1510625/cover.png";
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Published")]
        public string Published { get; set; }
        [Display(Name = "Volumes")]
        public int CountOfVolume { get; set; }
        [Display(Name = "Chapters")]
        public int CountOfChapters { get; set; }
        [Display(Name = "Score")]
        public float Score { get; set; }
        [Display(Name = "Ranked")]
        public int Ranked { get; set; }
        [Display(Name = "Popularity")]
        public int Popularity { get; set; }
        [Display(Name = "Genres")]
        public IEnumerable<string> Genres { get; set; }
         [Display(Name = "ReadLinks")]
        public IEnumerable<string> ReadLinks { get; set; }
         [Display(Name = "Status")]
        public MangaStatus Status { get; set; }
         [Display(Name = "Authors")]
        public IEnumerable<string> Authors { get; set; }
         [Display(Name = "Characters")]
        public List<MangaCharacter> Characters { get; set; }
        public List<Comment> Comments { get; set; }
         [Display(Name = "YearOfIssue")]
        public int YearOfIssue { get; set; }
         [Display(Name = "Language")]
        public Language Language { get; set; }
         [Display(Name = "Type")]
        public string Type { get; set; }
        public string Creator { get; set; }


        public static Manga CreateNew()
        {
            var manga = new Manga
            {
                Id = 0,
                OriginTitle = string.Empty,
                MangaImageUrl = string.Empty,
                Description = string.Empty,
                Published = string.Empty,
                CountOfVolume = 0,
                Score = 0,
                Ranked = 0,
                Popularity = 0,
                Genres = new List<string>(),
                Status = 0,
                Authors = new List<string>(),
                Characters = new List<MangaCharacter>(),
                YearOfIssue = 0
            };

            return manga;
        }

        public void CloneFrom(Manga otherManga)
        {
            this.OriginTitle = otherManga.OriginTitle;
            this.MangaImageUrl = otherManga.MangaImageUrl;
            this.Description = otherManga.Description;
            this.Published = otherManga.Published;
            this.CountOfChapters = otherManga.CountOfChapters;
            this.CountOfVolume = otherManga.CountOfVolume;
            this.Score = otherManga.Score;
            this.Ranked = otherManga.Ranked;
            this.Popularity = otherManga.Popularity;
            this.Genres = new List<string>(otherManga.Genres);
            this.Status = otherManga.Status;
            this.Authors = new List<string>(otherManga.Authors);
            this.Characters = new List<MangaCharacter>(otherManga.Characters);
            this.Comments = new List<Comment>(otherManga.Comments);
            this.YearOfIssue = otherManga.YearOfIssue;
            this.Type = otherManga.Type;
            this.ReadLinks = otherManga.ReadLinks;
        }
    }
    public enum MangaStatus
    {
        Publishing,
        Finished,
        Издано,
        Выходит
    }

    public enum Language
    {
        en,
        ru 
    }
    public enum MangaType
    {
        Manga,
        Manhwa,
        Manhua,
        Novel,
        One_shot,
        Doujinshi,
        Oel
    }
}
