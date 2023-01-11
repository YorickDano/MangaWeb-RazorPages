using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Models
{
    public class Manga
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
        public float Score { get; set; }
        public int Ranked { get; set; }
        public int Popularity { get; set; }
        public IEnumerable<string> Genres { get; set; }
        public MangaStatus Status { get; set; }
        public IEnumerable<string> Autors { get; set; }
        public List<MangaCharacter> Characters { get; set; }
        [Display(Name = "Year of issue")]
        public int YearOfIssue { get; set; }
        public Language Language { get; set; }
        public string Type { get; set; }


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
                Autors = new List<string>(),
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
            this.Autors = new List<string>(otherManga.Autors);
            this.Characters = new List<MangaCharacter>(otherManga.Characters);
            this.YearOfIssue = otherManga.YearOfIssue;
            this.Type = otherManga.Type;
        }
    }
    public enum MangaStatus
    {
        Publishing,
        Finished,
        Announced,
        Discontinued,
        Frozen,
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
