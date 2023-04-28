using HtmlAgilityPack;
using RestSharp;

namespace MangaWeb.APIClients.Services
{
    public class RequestExecutor : RestClientApi
    {
        private readonly RestClient RestClient;

        public RequestExecutor(string baseUrl) 
        {
            RestClient = new RestClient(baseUrl);
        }

        public async Task<IEnumerable<HtmlDocument>> SendRequestsAsync(IEnumerable<RestRequest> requests)
        {
            var htmlDocuments = new List<HtmlDocument>();

            foreach (var reqest in requests)
            {
                var response = await RestClient.ExecuteAsync(reqest);
                Console.WriteLine("Thread id: " + Environment.CurrentManagedThreadId);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response.Content);
                htmlDocuments.Add(htmlDocument);
            }

            return htmlDocuments;
        }

        public async Task<HtmlDocument> SendRequestAsync(RestRequest request)
        {
            var response = await RestClient.ExecuteAsync(request);
            var htmlDocument = new HtmlDocument();
            while (!response.IsSuccessful)
            {
                response = await RestClient.ExecuteAsync(request);           
            }
            htmlDocument.LoadHtml(response.Content);
            return htmlDocument;
        }
    }
}
