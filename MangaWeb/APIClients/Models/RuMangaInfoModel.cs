namespace MangaWeb.APIClients.Models
{
    public class RuMangaInfoModel
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Genre
        {
            public int id { get; set; }
            public string name { get; set; }
            public string russian { get; set; }
            public string kind { get; set; }
        }

        public class Image
        {
            public string original { get; set; }
            public string preview { get; set; }
            public string x96 { get; set; }
            public string x48 { get; set; }
        }

        public class Publisher
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class RatesScoresStat
        {
            public int name { get; set; }
            public int value { get; set; }
        }

        public class RatesStatusesStat
        {
            public string name { get; set; }
            public int value { get; set; }
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
            public List<string> english { get; set; }
            public List<string> japanese { get; set; }
            public List<object> synonyms { get; set; }
            public object license_name_ru { get; set; }
            public string description { get; set; }
            public string description_html { get; set; }
            public object description_source { get; set; }
            public object franchise { get; set; }
            public bool favoured { get; set; }
            public bool anons { get; set; }
            public bool ongoing { get; set; }
            public int thread_id { get; set; }
            public int topic_id { get; set; }
            public int myanimelist_id { get; set; }
            public List<RatesScoresStat> rates_scores_stats { get; set; }
            public List<RatesStatusesStat> rates_statuses_stats { get; set; }
            public List<object> licensors { get; set; }
            public List<Genre> genres { get; set; }
            public List<Publisher> publishers { get; set; }
            public object user_rate { get; set; }
        }


    }
}
