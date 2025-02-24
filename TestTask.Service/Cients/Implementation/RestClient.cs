using Newtonsoft.Json;
using TestTask.Service.Cients.Interfaces;
namespace TestTask.Service.Cients.Implementation
{
    public class RestClient : IClient
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _client;

        public RestClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _client = _httpClientFactory.CreateClient();
        }


        public async Task<IEnumerable<T>> GetTradesAsync<T>(string symbols)
        {
            try
            {
              
                string URL = $"https://api-pub.bitfinex.com/v2/trades/{symbols}/hist";
                var response = await _client.GetAsync(URL);
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(content);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
