
using Newtonsoft.Json;

namespace MangaWeb.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public string AuthorId { get; set; }    

        [JsonIgnore]
        public Manga Manga { get; set; }
        [JsonIgnore]
        public MangaCharacter Character { get; set; }
    }
}
