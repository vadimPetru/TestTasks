using Newtonsoft.Json.Linq;

namespace TestHQ;

public class Trade
{
    /// <summary>
    /// Валютная пара
    /// </summary>
    public string Pair { get; set; }

    /// <summary>
    /// Цена трейда
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Объем трейда
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Направление (buy/sell)
    /// </summary>
    public string Side { get; set; }

    /// <summary>
    /// Время трейда
    /// </summary>
    public DateTimeOffset Time { get; set; }


    /// <summary>
    /// Id трейда
    /// </summary>
    public string Id { get; set; }

    #region Handler для метчинг модели trade
    /// <summary>
    ///  Мэтчинг данных
    /// </summary>
    /// <param name="array">Коллекция данных из внешнего апи</param>
    public static void HandleTrades(JArray array)
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
    #endregion

    #region Handlers для оработки допалнительной информации приходящей из Websoket
    public static bool IsMessageType(JToken message)
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
    #endregion

}
