namespace MangaWeb.Models
{
    public class MangaCharacter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        public string Description { get; set; }
        public List<string> ImagesUrls { get; set; }
        public FullManga FullManga { get; set; }

        public MangaCharacter(string name, string imageUrl, bool isMain)
        {
            Name = name;
            ImageUrl = imageUrl;
            IsMain = isMain;    
        }

        public MangaCharacter(string name, string imageUrl, bool isMain, string description) : this(name, imageUrl,isMain)
        {
            Description = description; 
        }

        public MangaCharacter(string name, string imageUrl, bool isMain, string description, List<string> imagesUrls) : this(name,imageUrl,isMain,description)
        {        
            ImagesUrls = imagesUrls;
        }
    }
}
