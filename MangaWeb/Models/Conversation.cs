namespace MangaWeb.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string FirstUserName { get; set; }
        public string FirstUserImageSrc { get; set; }
        public string SecondUserName { get; set; }
        public string SecondUserImageSrc { get; set; }
        public DateTime Created { get; set; }
        public List<Message> Messages { get; set; }
    }
}
