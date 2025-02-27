using ConnectorTest;
using Newtonsoft.Json.Linq;
using TestHQ;
using TestTask.Models.Models;
using TestTask.Service.Cients.Implementation;
using TestTask.Service.Cients.Interfaces;

namespace TestTask.Service.Connector.Implementation;

public class Connector : ITestConnector
{
    private readonly IRestClient _client;
    private readonly IWebSocketClient _webSocketClient;
    private readonly Dictionary<SubscriptionKey, Subscription>? _tradeSubscriptions;
    private readonly Dictionary<string, int>? _candleSubscriptions;

    public event Action<Trade>? NewBuyTrade;
    public event Action<Trade>? NewSellTrade;
    public event Action<Candle>? CandleSeriesProcessing;

    public Connector(IRestClient client)    
    {
        _client = client;
        _tradeSubscriptions = new();
        _candleSubscriptions = new();
        _webSocketClient = new WebSocketClient("wss://api-pub.bitfinex.com/ws/2");
        _webSocketClient.OnMessageReceived += OnWebSocketMessageReceived!;
    }

    public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount = 0)
    {
        try
        {
            var result = await _client.GetTradesAsync(pair);
            return result;

        }
        catch (Exception Ex)
        {
            throw new Exception(Ex.Message);
        }
    }

    public async Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair,
        int periodInSec,
        DateTimeOffset? from,
        DateTimeOffset? to = null,
        long? count = 0
        )
    {
        try
        {
            var period = FrameConvert.ConvertPeriodIntSecToString(periodInSec);
            string candle = $"trade:{period}:{pair}";
            long startTime = from?.ToUnixTimeMilliseconds() ?? 0;
            long endDate = to?.ToUnixTimeMilliseconds() ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var result = await _client.GetCandleAsync(pair, candle, startTime, endDate, count);
            return result;

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<Ticker> GetTicker(string symbol)
    {
        try
        {
            var result = await _client.GetTickerAsync(symbol);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

   /// <summary>
   /// Метод для прослушивания соединения
   /// </summary>
   /// <returns>Задачу</returns>
    public async Task Processing()
    {
        await _webSocketClient.ListenForMessageAsync();
    }

    /// <summary>
    /// Подключение WebSoket
    /// </summary>
    /// <returns>Задачу</returns>
    public async Task ConnectAsync()
    {
        await _webSocketClient.ConnectAsync();
    }

    private void OnWebSocketMessageReceived(object sender, string message)
    {
        try
        {

            var messageJToken = JToken.Parse(message);

            if (_webSocketClient.IsHeartbeat(messageJToken))
            {
                Console.WriteLine($"IsHeartbeatMessage: {messageJToken}");
            }

            else if (Trade.IsMessageType(messageJToken))
            {
                Console.WriteLine($"MessageTypeHandler: {messageJToken}");
            }

            else if (messageJToken is JArray)
            {
                _webSocketClient.HandleData(messageJToken);
            }

            else if (messageJToken["event"].ToObject<string>() == "subscribed"
                  && messageJToken["channel"].ToObject<string>() == "trades")
            {

                var key = new SubscriptionKey(messageJToken["symbol"]!.ToObject<string>()!,
                    messageJToken["channel"]!.ToObject<string>()!
                    );

                _tradeSubscriptions.TryGetValue(key,
                    out var result);

                result.ChangeModel(string.Empty,messageJToken["chanId"]!.ToObject<int>()!);
                
            }
            else if (messageJToken["event"].ToObject<string>() == "subscribed"
                && messageJToken["channel"].ToObject<string>() == "candles")
            {

                var key = new SubscriptionKey(messageJToken["symbol"]!.ToObject<string>()!,
                    messageJToken["channel"]!.ToObject<string>()!
                    );

                _candleSubscriptions.TryGetValue(key,
                    out var result);

                result.ChangeModel(string.Empty, messageJToken["chanId"]!.ToObject<int>()!);

            }
            else if (messageJToken["event"] != null)
            {
               _webSocketClient.HandleEvent(messageJToken);
            }
        }catch(Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
      
    }
    public async Task SubscribeTrades(string symbol, int maxCount = 100)
    {
        var key = new SubscriptionKey(symbol, "trades");
        if (!_tradeSubscriptions.ContainsKey(key))
        {
            _tradeSubscriptions[key] = Subscription.CreateInstance(0,"trades",maxCount);
            var message = new SubscribeTradeMessage("subscribe",
                "trades",
                symbol
            );
            await _webSocketClient.SendEventAsync(message);
        }
    }

    public async Task UnsubscribeTrades(string symbol)
    {
        var key = new SubscriptionKey(symbol, "trades");
      
        if (_tradeSubscriptions.ContainsKey(key))
        {
            _tradeSubscriptions.TryGetValue(key, out var value);
            var message = new UnSubscribeTradeMessage(@event:"unsubscribe",
              chanId: value.ChanId
            );
           
            await _webSocketClient.SendEventAsync(message);
            _tradeSubscriptions.Remove(key);
        }
    }

    public async Task SubscribeCandles(string pair,
        int periodInSec,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        long? count = 0
        )
    {
        string key = $"{pair}_{periodInSec}";
        if (!_candleSubscriptions.ContainsKey(key))
        {
            _candleSubscriptions[key] = 0;
          

            var period = FrameConvert.ConvertPeriodIntSecToString(periodInSec);
            string candle = $"trade:{period}:{pair}";


            var message = new SubscribeCandleMessage("subscribe",
                "candles", candle);
            await _webSocketClient.SendEventAsync(message
                 );
        }
    }


    public Task UnsubscribeCandles(string pair)
    {
        var keys = _candleSubscriptions.Keys
            .Where(candle => candle.StartsWith(pair))
            .ToList();

        foreach (var keyRemove in keys)
        {
            

            await _webSocketClient.SendEventAsync("unsubscribeCandles", new { pair });
            _candleSubscriptions.Remove(keyRemove);
        }
    }

    public void Dispose()
    {
        _webSocketClient?.Dispose();
    }


  
}
