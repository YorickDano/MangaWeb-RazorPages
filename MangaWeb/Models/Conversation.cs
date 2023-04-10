using MangaWeb.Areas.Identity.Data;

namespace MangaWeb.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string FirstUserId { get; set; }
        public string SecondUserId { get; set; }
        public MangaWebUser FirstUser { get; set; }
        public MangaWebUser SecondUser { get; set; }
        public DateTime Created { get; set; }
        public List<Message> Messages { get; set; }
    }
}
