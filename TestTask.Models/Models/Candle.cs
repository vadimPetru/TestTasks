using Newtonsoft.Json.Linq;

namespace TestHQ;

public class Candle
{
    /// <summary>
    /// Валютная пара
    /// </summary>
    public string Pair { get; set; }

    /// <summary>
    /// Цена открытия
    /// </summary>
    public decimal OpenPrice { get; set; }

    /// <summary>
    /// Максимальная цена
    /// </summary>
    public decimal HighPrice { get; set; }

    /// <summary>
    /// Минимальная цена
    /// </summary>
    public decimal LowPrice { get; set; }

    /// <summary>
    /// Цена закрытия
    /// </summary>
    public decimal ClosePrice { get; set; }


    /// <summary>
    /// Partial (Общая сумма сделок)
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Partial (Общий объем)
    /// </summary>
    public decimal TotalVolume { get; set; }

    /// <summary>
    /// Время
    /// </summary>
    public DateTimeOffset OpenTime { get; set; }

    #region Handler Для метчинга данных
    /// <summary>
    /// Обработчик для матчинга данных
    /// </summary>
    /// <param name="array">Массив данных пришедший с внешнего апи</param>
    public static void HandleCandle(JArray array)
    {
        if (array.Count == 6)
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

        foreach (var candle in array)
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
    # endregion

}
