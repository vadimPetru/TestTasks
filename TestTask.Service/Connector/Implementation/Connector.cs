using ConnectorTest;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using TestHQ;
using TestTask.Models.Enums;
using TestTask.Models.Models;
using TestTask.Models.Models.Event;
using TestTask.Models.ResultObject;
using TestTask.Service.Cients.Implementation;
using TestTask.Service.Cients.Interfaces;
using TestTask.Service.HandleProcessing.Interface;

namespace TestTask.Service.Connector.Implementation;

public class Connector : ITestConnector
{
    private readonly IRestClient _client;
    private readonly IWebSocketClient _webSocketClient;
    private readonly IHandler _handler;

    /// <summary>
    /// Коллекция для подписок Trades
    /// </summary> 
    private readonly Dictionary<SubscriptionKey, Subscription>? _tradeSubscriptions;
    /// <summary>
    /// Коллекция для подписок Candles
    /// </summary>
    private readonly Dictionary<SubscriptionKey, Subscription>? _candleSubscriptions;
    /// <summary>
    /// Для канала Trade
    /// </summary>  
    public event Action<Trade>? NewBuyTrade;
    public event Action<Trade>? NewSellTrade;
    /// <summary>
    /// Для канала Candle
    /// </summary>  
    public event Action<Candle>? CandleSeriesProcessing;
    /// <summary>
        /// События для дополнительных сообщений
        /// </summary>
    public event Func<JToken, Heartbeat> OnHeartbeatMessage;
    public event Action<TradeExecutedEventArgs> TradeExecuted;
    public event Action<TradeUpdatedEventArgs> TradeUpdated;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="client">Нискоуровневая реализация RestClient</param>
    public Connector(IRestClient client, IHandler handler)
    {
        _client = client;
        _tradeSubscriptions = new();
        _candleSubscriptions = new();
        _webSocketClient = new WebSocketClient("wss://api-pub.bitfinex.com/ws/2");
        _webSocketClient.OnMessageReceived += OnWebSocketMessageReceived!;
        _webSocketClient.OnError += OnErrorHandler!;
        _handler = handler;
        _handler.OnTradeMapping += OnTradeSide!;
    }

    private void OnTradeSide(object sender , Trade trade)
    {
        if(trade.Side is "sell")
        {
            NewBuyTrade?.Invoke(trade);
            return;
        }
        NewSellTrade?.Invoke(trade);
    }

    /// <summary>
    /// Получение Торговли
    /// </summary>
    /// <param name="pair">Пара</param>
    /// <param name="maxCount">Максимальное количество</param>
    /// <returns></returns>
    public async Task<Result<IEnumerable<Trade>>> GetNewTradesAsync(string pair, int maxCount = 0)
    {
        try
        {
            var result = await _client.GetTradesAsync(pair);
            return result;

        }
        catch (Exception Ex)
        {
            return Result.Failure<IEnumerable<Trade>>(
                new Error("01", Ex.Message));
        }
    }
    /// <summary>
    /// Получения свечей
    /// </summary>
    /// <param name="pair">Пара валют</param>
    /// <param name="periodInSec">Время в секундах</param>
    /// <param name="from">Время начала</param>
    /// <param name="to">Время конца</param>
    /// <param name="count">Количество</param>
    /// <returns>Обьект результата</returns>    
    public async Task<Result<IEnumerable<Candle>>> GetCandleSeriesAsync(string pair,
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
            return Result.Failure<IEnumerable<Candle>>(
                new Error("02", ex.Message));
        }
    }
    /// <summary>
    /// Получение Ticker
    /// </summary>
    /// <param name="symbol">Параметр валюты</param>
    /// <returns>Обьект результата</returns>
    public async Task<Result<Ticker>> GetTicker(string symbol)
    {
        try
        {
            var result = await _client.GetTickerAsync(symbol);
            return result;
        }
        catch (Exception ex)
        {

            return Result.Failure<Ticker>(
                new Error("03", ex.Message));
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

    #region MaunHanlder Erros
    private void OnErrorHandler(object sender, string message)
    {
        throw new Exception(message);
    }
    #endregion

    #region MainHandler
    private void OnWebSocketMessageReceived(object sender, string message)
    {
        try
        {
            var messageJToken = JToken.Parse(message);

            if (_webSocketClient.IsHeartbeat(messageJToken))
            {
                OnHeartbeatMessage?.Invoke(messageJToken);
            }

            else if (messageJToken is JArray array && array.Count == 3)
            {
                MessageTypeProcessed(messageJToken);
            }

            else if (messageJToken is JArray)

            {
                _handler.HandleData(messageJToken);
            }

            else if (IsSubscribedTrades(messageJToken))
            {
                ChangeChanIdForTrades(messageJToken);

            }
            else if (IsSubscribedCandles(messageJToken))
            {
                ChangeChanIdForCandles(messageJToken);

            }
            else if (messageJToken[nameof(JsonName.@event)] != null)
            {
                _handler.HandleEvent(messageJToken);
            }
        }
        catch (Exception ex)
        {
            OnErrorHandler(this, ex.Message);
        }

    }

    #region Получения сообщения о подписке на Candles для замены индификатора chanId
        private static bool IsSubscribedCandles(JToken messageJToken) => messageJToken[nameof(JsonName.@event)].ToObject<string>() == nameof(EventType.subscribed)
                && messageJToken[nameof(JsonName.channel)].ToObject<string>() == nameof(ChannelName.candles);
        #endregion

    #region Получения сообщения о подписке на Trades для замены индификатора chanId
    private static bool IsSubscribedTrades(JToken messageJToken) => messageJToken[nameof(JsonName.@event)].ToObject<string>() == nameof(EventType.subscribed)
            && messageJToken[nameof(JsonName.channel)].ToObject<string>() == nameof(ChannelName.trades);
    #endregion

    #region Изменения chanId для Trades
    private void ChangeChanIdForTrades(JToken messageJToken)
    {

        var key = new SubscriptionKey(messageJToken[nameof(JsonName.symbol)]!.ToObject<string>()!,
            messageJToken[nameof(JsonName.channel)]!.ToObject<string>()!
            );

        _tradeSubscriptions.TryGetValue(key,
            out var result);

        result.ChangeModel(string.Empty, messageJToken[nameof(JsonName.chanId)]!.ToObject<int>()!);
    }
    #endregion

    #region Изменения chanId для Candles
    private void ChangeChanIdForCandles(JToken messageJToken)
    {
        var key = new SubscriptionKey(messageJToken[nameof(JsonName.key)]!.ToObject<string>()
                                                            .Split(':')
                                                            .Last(),
            messageJToken[nameof(JsonName.channel)]!.ToObject<string>()!
            );

        _candleSubscriptions.TryGetValue(key,
            out var result);

        result.ChangeModel(string.Empty, messageJToken[nameof(JsonName.chanId)]!.ToObject<int>()!);
    }
    #endregion
    #endregion

    #region Handlers для обработки допалнительной информации
    private void MessageTypeProcessed(JToken message)
    {

        int channelId = message[0].ToObject<int>();
        string msgType = message[1].ToObject<string>();
        var tradeData = message[2];

        var _handlers = new ConcurrentDictionary<string, Action<int, object>>();

        _handlers.TryAdd("te", HandlerTradeExecuted);
        _handlers.TryAdd("tu", HandlerTradeUpdated);


        if (_handlers.TryGetValue(msgType, out var handler))
        {
            handler(channelId, tradeData);

        }


    }

    private void HandlerTradeExecuted(int channelId, object tradeData)
    {
        TradeExecuted?.Invoke(new TradeExecutedEventArgs(channelId, tradeData));
    }
    private void HandlerTradeUpdated(int channelId, object tradeData)
    {
        TradeUpdated?.Invoke(new TradeUpdatedEventArgs(channelId, tradeData));
    }
    #endregion

    #region Trades
    public async Task SubscribeTrades(string symbol, int maxCount = 100)
    {
        var key = new SubscriptionKey(symbol, nameof(ChannelName.trades));
        if (!_tradeSubscriptions.ContainsKey(key))
        {
            _tradeSubscriptions[key] = Subscription.CreateInstance(0,
                nameof(ChannelName.trades),
                maxCount
                );

            var message = new SubscribeTradeMessage(nameof(EventType.subscribe),
               nameof(ChannelName.trades),
                symbol
            );
            await _webSocketClient.SendEventAsync(message);
        }
    }

    public async Task UnsubscribeTrades(string symbol)
    {
        var key = new SubscriptionKey(symbol, nameof(ChannelName.trades));

        if (_tradeSubscriptions.ContainsKey(key))
        {
            _tradeSubscriptions.TryGetValue(key, out var value);
            var message = new UnSubscribeTradeMessage(nameof(EventType.unsubscribe),
              chanId: value.ChanId
            );

            await _webSocketClient.SendEventAsync(message);
            _tradeSubscriptions.Remove(key);
        }
    }
    #endregion

    #region Candle
    public async Task SubscribeCandles(string pair,
        int periodInSec,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        long? count = 0
        )
    {

        var key = new SubscriptionKey(pair, nameof(ChannelName.candles));
        if (!_candleSubscriptions.ContainsKey(key))
        {
            _candleSubscriptions[key] = Subscription.CreateInstance(0, nameof(ChannelName.candles), 100);
            var period = FrameConvert.ConvertPeriodIntSecToString(periodInSec);

            string candle = $"trade:{period}:{pair}";


            var message = new SubscribeCandleMessage(nameof(EventType.subscribe),
                nameof(ChannelName.candles),
                candle
                );

            await _webSocketClient.SendEventAsync(message);
        }
    }


    public async Task UnsubscribeCandles(string pair)
    {
        var keys = _candleSubscriptions.Keys
            .Where(candle => candle.symbol.StartsWith(pair))
            .ToList();

        foreach (var keyRemove in keys)
        {
            var chanId = _candleSubscriptions[keyRemove].ChanId;

            var message = new UnSubscribeCandleMessage(nameof(EventType.unsubscribe),
                chanId
                );


            await _webSocketClient.SendEventAsync(message);
            _candleSubscriptions.Remove(keyRemove);
        }
    }
#endregion

    public void Dispose()
    {
        _webSocketClient?.Dispose();
    }



}
