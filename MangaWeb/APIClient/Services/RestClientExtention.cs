using RestSharp;

namespace MangaWeb.APIClient.Services
{
    public static class RestClientExtention
    {
        public static void ChangeBaseUrlOn(this RestClient restClient,string url)
        {
            restClient.Options.BaseUrl = new Uri(url);
        } 
    }
}
