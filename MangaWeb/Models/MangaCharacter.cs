using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace MangaWeb.Models
{
    public class MangaCharacter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
        [Display(Name = "Is main?")]
        public bool IsMain { get; set; }
        public string Description { get; set; }
        public List<string> ImagesUrls { get; set; }
        public List<Comment> Comments { get; set; } 
        [JsonIgnore]
        public Manga Manga { get; set; }

        public MangaCharacter() { }

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
