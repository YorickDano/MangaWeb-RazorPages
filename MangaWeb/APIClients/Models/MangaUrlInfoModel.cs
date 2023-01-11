namespace MangaWeb.APIClients.Models
{
    public class MangaUrlInfoModel
    {
        public class Datum
        {
            public Node node { get; set; }
        }

        public class MainPicture
        {
            public string medium { get; set; }
            public string large { get; set; }
        }

        public class Node
        {
            public int id { get; set; }
            public string title { get; set; }
            public MainPicture main_picture { get; set; }
        }

        public class Paging
        {
            public string next { get; set; }
        }

        public class Root
        {
            public List<Datum> data { get; set; }
            public Paging paging { get; set; }
        }

    }
}
