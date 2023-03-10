namespace MangaWeb.APIClients.Models
{
    public class MangaInfoModel
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Author
        {
            public Node node { get; set; }
            public string role { get; set; }
        }

        public class Genre
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class MainPicture
        {
            public string medium { get; set; }
            public string large { get; set; }
        }

        public class Node
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
        }

        public class Root
        {
            public int id { get; set; }
            public string title { get; set; }
            public MainPicture main_picture { get; set; }
            public string start_date { get; set; }
            public string end_date { get; set; }
            public string synopsis { get; set; }
            public double mean { get; set; }
            public int rank { get; set; }
            public int popularity { get; set; }
            public List<Genre> genres { get; set; }
            public DateTime created_at { get; set; }
            public string media_type { get; set; }
            public string status { get; set; }
            public int num_volumes { get; set; }
            public int num_chapters { get; set; }
            public List<Author> authors { get; set; }
        }
    }
}
