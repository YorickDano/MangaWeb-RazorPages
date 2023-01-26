namespace MangaWeb.APIClients.Models
{
    public class RuMangaUrlInfoModel
    {
        public class Image
        {
            public string original { get; set; }
            public string preview { get; set; }
            public string x96 { get; set; }
            public string x48 { get; set; }
        }

        public class Root
        {
            public int id { get; set; }
            public string name { get; set; }
            public string russian { get; set; }
            public Image image { get; set; }
            public string url { get; set; }
            public string kind { get; set; }
            public string score { get; set; }
            public string status { get; set; }
            public int volumes { get; set; }
            public int chapters { get; set; }
            public string aired_on { get; set; }
            public string released_on { get; set; }
        }


    }
}
