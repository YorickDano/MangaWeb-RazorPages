using System.ComponentModel.DataAnnotations;

namespace MangaWeb.Models
{
    public class MangaRead
    {
        [Key]
        public int Id { get; set; }
        public List<MangaReadPage> Pages { get; set; }
        public string Title { get; set; }
        public int VolumeNumber { get; set; }
        public int ChapterNumber { get; set; }

       
    }
}
