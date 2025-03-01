using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace TestHQ;

public class Trade
{
    /// <summary>
    /// Id трейда
    /// </summary>
    public string Id { get; set; }
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

    public Action<Trade> OnTradeMapping;
    #region Handler для метчинг модели trade
    /// <summary>
    ///  Мэтчинг данных
    /// </summary>
    /// <param name="array">Коллекция данных из внешнего апи</param>
    public void HandleTrades(JArray array)
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
            OnTradeMapping?.Invoke(this);
        }
    }
    #endregion



     
}
