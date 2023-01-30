using MangaWeb.Areas.Identity.Data;

namespace MangaWeb.Models.ForumModels
{
    public class Post
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImgSrc { get; set; }
        public DateTime Date { get; set; }
        public int TopicID { get; set; }
        public Topic Topic { get; set; }
    }
}
