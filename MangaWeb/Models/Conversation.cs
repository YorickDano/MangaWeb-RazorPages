namespace MangaWeb.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string FirstUserName { get; set; }
        public string SecondUserName { get; set; }
        public DateTime Created { get; set; }
    }
}
