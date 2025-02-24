using System.Net.Http;

namespace TestTask.Service.Cients.Implementation
{
    public class RestClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RestClient(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;
    }
}
