using Newtonsoft.Json;
using TestHQ;
using TestTask.Models.Models;
using TestTask.Service.Cients.Interfaces;
namespace TestTask.Service.Cients.Implementation
{
    public class RestClient : IRestClient
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

        public async Task<List<Candle>> GetCandleAsync(
            string pair,
            string candle,
            long startTime,
            long endTime,
            long? count = 0
            )
        {
            string url = $"https://api-pub.bitfinex.com/v2/candles/{candle}/hist?start={startTime}&end={endTime}&limit={count}";

            var jsonResponse = await _client.GetAsync(url);
            var content = await jsonResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<List<List<decimal>>>(content);
            var candles = new List<Candle>();

            foreach(var arrayCandle in response)
            {
                candles.Add(new Candle()
                {
                    Pair = pair,
                    ClosePrice = arrayCandle[2],
                    HighPrice = arrayCandle[3],
                    LowPrice = arrayCandle[4],
                    OpenPrice = arrayCandle[1],
                    OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)arrayCandle[0]),
                    TotalPrice= arrayCandle[5] * arrayCandle[2],
                    TotalVolume = arrayCandle[5]
                });
            }

            return candles;
        }

        public async Task<Ticker> GetTickerAsync(string symbol)
        {
            string url = $"https://api-pub.bitfinex.com/v2/ticker/{symbol}";
            var jsonResponse = await _client.GetAsync(url);
            var content = await jsonResponse.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<Ticker>(content);
            return response;
        }
    }
}
