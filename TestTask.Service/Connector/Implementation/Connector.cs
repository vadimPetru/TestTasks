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
    private readonly Dictionary<string, int>? _tradeSubscriptions;
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

    // Просушивание WebSoketa
    public async Task Processing()
    {
        await _webSocketClient.ListenForMessageAsync();
    }
    /// <summary>
    /// Подключение WebSoket
    /// </summary>
    /// <returns></returns>
    public async Task ConnectAsync()
    {
        await _webSocketClient.ConnectAsync();
    }

    private void OnWebSocketMessageReceived(object sender, string message)
    {
        try
        {
            var messageJToken = JToken.Parse(message);

            if (IsHeartbeat(messageJToken))
            {
                Console.WriteLine($"IsHeartbeatMessage: {messageJToken}");
            }

            else if (IsMessageType(messageJToken))
            {
                Console.WriteLine($"MessageTypeHandler: {messageJToken}");
            }

            else if (messageJToken is JArray)
            {
                HandleData(messageJToken);
            }

            else if (messageJToken["event"] != null)
            {
                HandleEvent(messageJToken);
            }
        }catch(Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
      
    }
    private bool IsHeartbeat(JToken message)
    {
        return (message is JArray array && array.Count == 0) ||
        (message is JArray hbArray 
        && hbArray.Count == 2 && hbArray[1].ToString() == "hb");
    }

    private bool IsMessageType(JToken message)
    {
        if (message is JArray array && array.Count == 3)
        {


            int channelId = message[0].ToObject<int>();
            string msgType = message[1].ToObject<string>();
            var tradeData = message[2];

            var handlers = new Dictionary<string, Action<int, object>>()
            {
                {"te", HandlerTradeExecuted },
                {"tu" , HandlerTradeUpdated },
                {"fte" , HandleFundingTradeExecuted },
                {"ftu" , HandlerfundingTradeUpdated },
            };


            if (handlers.TryGetValue(msgType, out var handler))
            {
                handler(channelId, tradeData);
                return true;
            }
        }
            Console.WriteLine($"Другие данные");
            return false;
    }

    private static void HandlerTradeExecuted(int channelId, object tradeData)
    {
        Console.WriteLine($"{channelId} - {tradeData}");

    }
    private static void HandlerTradeUpdated(int channelId, object tradeData)
    {
        Console.WriteLine($"{channelId} - {tradeData}");

    }
    private static void HandleFundingTradeExecuted(int channelId, object tradeData)
    {
        Console.WriteLine($"{channelId} - {tradeData}");

    }
    private static void HandlerfundingTradeUpdated(int channelId, object tradeData)
    {
        Console.WriteLine($"{channelId} - {tradeData}");

    }
    private void HandleEvent(JToken eventMessage)
    {
        string eventType = eventMessage["event"]?.ToString()!;

        switch (eventType)
        {
            case "info":
                Console.WriteLine("Подключение Успешно");
                break;
            case "subscribed":
                Console.WriteLine("Подписка на канал");
                break;
            case "unsubscribed":
                Console.WriteLine("Подписка на канал");
                break;
            default:
                Console.WriteLine("Неизвестное событие");
                break;
        }
    }

    private void HandleData(JToken messageData)
    {
        var channelId = messageData[0].ToObject<int>();
        var array = messageData[1] as JArray;

        if(array is null || array.Count == 0)
        {
            Console.WriteLine("Массив пустой");
        }

        if (array.Count == 6)
        {
            HandleCandle(array);
            return;
        }
        var firstItem = array[0] as JArray;

        if(firstItem.Count == 4)
        {
            HandleTrades(array);
        }
        else
        {
            HandleCandle(array);
        }

       
    }

    private void HandleCandle(JArray array)
    {
        if(array.Count == 6)
        {
            var item = new Candle()
            {
                ClosePrice = array[2].ToObject<decimal>(),
                HighPrice = array[3].ToObject<decimal>(),
                LowPrice = array[4].ToObject<decimal>(),
                OpenPrice = array[1].ToObject<decimal>(),
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)array[0]),
                TotalPrice = array[5].ToObject<decimal>() * array[2].ToObject<decimal>(),
                TotalVolume = array[5].ToObject<decimal>()
            };
            Console.WriteLine($"ClosePrice:{item.ClosePrice} , HighPrice:{item.HighPrice}");
            return;
        }
        
        foreach(var candle in array)
        {
            var item = new Candle()
            {
                ClosePrice = candle[2].ToObject<decimal>(),
                HighPrice = candle[3].ToObject<decimal>(),
                LowPrice = candle[4].ToObject<decimal>(),
                OpenPrice = candle[1].ToObject<decimal>(),
                OpenTime = DateTimeOffset.FromUnixTimeMilliseconds((long)candle[0]),
                TotalPrice = candle[5].ToObject<decimal>() * candle[2].ToObject<decimal>(),
                TotalVolume = candle[5].ToObject<decimal>()
            };
            Console.WriteLine($"ClosePrice:{item.ClosePrice} , HighPrice:{item.HighPrice}");
        }
    }

    private void HandleTrades(JArray array)
    {
        foreach (var trade in array)
        {
            var item = new Trade()
            {
                Id = trade[0].ToString(),
                Time = DateTimeOffset.FromUnixTimeMilliseconds((long)trade[1]),
                Amount = trade[2].ToObject<decimal>(),
                Price = trade[3].ToObject<decimal>(),
                Side = trade[2].ToObject<decimal>() < 0 ? "sell" : "buy"
            };

            Console.WriteLine($"Item:{item}");
        }
    }

    public async Task SubscribeTrades(string symbol, int maxCount = 100)
    {
        if (!_tradeSubscriptions.ContainsKey(symbol))
        {
            _tradeSubscriptions[symbol] = maxCount;

            var message = new SubscribeTradeMessage("subscribe",
                "trades",
                symbol
            );

            await _webSocketClient.SendEventAsync(message);
        }
    }

    public async Task UnsubscribeTrades(string symbol)
    {
        if (_tradeSubscriptions.ContainsKey(symbol))
        {
            _tradeSubscriptions.Remove(symbol);
            var message = new UnSubscribeTradeMessage("unsubscribe"
                );
           
            await _webSocketClient.SendEventAsync(message);
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
            _candleSubscriptions[key] = periodInSec;
            //TODO: Повторяеться код

            var period = FrameConvert.ConvertPeriodIntSecToString(periodInSec);
            string candle = $"trade:{period}:{pair}";


            var message = new SubscribeCandleMessage("subscribe",
                "candles", candle);
            await _webSocketClient.SendEventAsync(message
                 );
        }
    }


    //public Task UnsubscribeCandles(string pair)
    //{
    //    var keys = _candleSubscriptions.Keys
    //        .Where(candle => candle.StartsWith(pair))
    //        .ToList();

    //    foreach (var keyRemove in keys)
    //    {
    //        _candleSubscriptions.Remove(keyRemove);
    //        await _webSocketClient.SendEventAsync("unsubscribeCandles", new { pair });
    //    }
    //}

    public void Dispose()
    {
        _webSocketClient?.Dispose();
    }
}
