using Newtonsoft.Json;

namespace MangaWeb.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string UserNameFrom { get; set; }
        public string UserNameTo { get; set; }
        public string Body { get; set; }
        public DateTime DateTime { get; set; }

        [JsonIgnore]
        public Conversation Conversation { get; set; }
    }
}
