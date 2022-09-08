namespace MangaWeb.Models
{
    public class MangaCharacter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        public FullManga FullManga { get; set; }

        public MangaCharacter(string name, string imageUrl, bool isMain)
        {
            Name = name;
            ImageUrl = imageUrl;
            IsMain = isMain;
        }
    }
}
