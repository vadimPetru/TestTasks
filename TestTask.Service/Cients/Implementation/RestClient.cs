using Newtonsoft.Json;
using TestHQ;
using TestTask.Models.Models.Response;
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


        public async Task<List<Trade>> GetTradesAsync(string symbols)
        {
            try
            {

                string URL = $"https://api-pub.bitfinex.com/v2/trades/{symbols}/hist";
                var jsonResponse = await _client.GetAsync(URL);
                var content = await jsonResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<List<List<decimal>>>(content);
                var trades = new List<Trade>();

                foreach (var trade in response)
                {
                    trades.Add(new Trade()
                    {
                        Id = trade[0].ToString(),
                        Amount = trade[2],
                        Price = trade[3],
                        Pair = symbols,
                        Side = trade[2] < 0 ? "sell" : "buy",
                        Time = DateTimeOffset.FromUnixTimeMilliseconds((long)trade[1])
                    }
                        );
                }

                return trades;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<>
    }
}
