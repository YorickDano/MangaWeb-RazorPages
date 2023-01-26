using RestSharp;
using System.Security.Policy;

namespace MangaWeb.APIClients.Services
{
    public static class RestClientExtention
    {
        private static string BaseUrl { get; set; }
        private static string EndAdd { get; set; }

        public static void ChangeBaseUrlOn(this RestClient restClient, string url)
        {
            BaseUrl = url;
            restClient.Options.BaseUrl = new Uri(url, UriKind.RelativeOrAbsolute);
        }

        public static void AddAtEndBaseUrl(this RestClient restClient, string add)
        {
            EndAdd = add;
            restClient.Options.BaseUrl = new Uri(BaseUrl + add);
            BaseUrl = restClient.Options.BaseUrl.ToString();
        }

        public static void RemoveLastAddBaseUrl(this RestClient restClient)
        {
            restClient.Options.BaseUrl = new Uri(BaseUrl.Replace(EndAdd, ""));
            BaseUrl = restClient.Options.BaseUrl.ToString();
        }
    }
}
