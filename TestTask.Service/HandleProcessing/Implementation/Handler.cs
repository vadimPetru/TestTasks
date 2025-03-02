using Newtonsoft.Json.Linq;
using TestHQ;
using TestTask.Models.Enums;
using TestTask.Service.HandleProcessing.Interface;

namespace TestTask.Service.HandleProcessing.Implementation
{
    public sealed class Handler : IHandler
    {
        public event Action<string> OnError;
        public event Action<object> OnInfoMessage;
        public event Action<object> OnSubscribeMessage;
        public event Action<object> OnUnSubscribeMessage;
        public event EventHandler<Trade> OnTradeMapping;
        public event EventHandler<Candle> OnCandleMapping;

        public void HandleEvent(JToken eventMessage)
        {
            string eventType = eventMessage[nameof(JsonName.@event)]?.ToString()!;
            string chanId = eventMessage[nameof(JsonName.chanId)]?.ToString()!;

            switch (eventType)
            {
                case nameof(EventType.info):
                    OnInfoMessage?.Invoke(eventMessage);
                    break;
                case nameof(EventType.subscribed):
                    OnSubscribeMessage?.Invoke(eventMessage);
                    break;
                case nameof(EventType.unsubscribed):
                    OnSubscribeMessage?.Invoke(eventMessage);
                    break;
                default:
                    OnError?.Invoke(eventType);
                    break;
            }
        }
        public void HandleData(JToken messageData)
        {
            var additionalArray = messageData as JArray;
            if(additionalArray.Count == 3)
            {
                if(additionalArray[1].ToObject<string>() == "te" ||
                    additionalArray[1].ToObject<string>() == "tu")
                {
                    var list = additionalArray[2] as JArray;
                    HandleTrades(list);
                    return;
                }
            }
            var array = messageData[1] as JArray;
            var channelId = messageData[0].ToObject<int>();
           
            

            if (array is null || array.Count == 0)
            {
                OnError?.Invoke("Пустой массив");
                return;
            }

            if (array.Count == 6)
            {
                HandleCandle(array);
                return;
            }

            var firstItem = array[0] as JArray;
            if (firstItem.Count == 4)
            {
                HandleTrades(array);
                return;
            }
            else
            {
                HandleCandle(array);
                return;
            }
        }

 
        #region Handler для метчинг модели Trade
        /// <summary>
        ///  Мэтчинг данных
        /// </summary>
        /// <param name="array">Коллекция данных из внешнего апи</param>
        public void HandleTrades(JArray array)
        {
            if(array.Count == 4)
            {
                var item = new Trade()
                {
                    Id = array[0].ToString(),
                    Time = DateTimeOffset.FromUnixTimeMilliseconds((long)array[1]),
                    Amount = array[2].ToObject<decimal>(),
                    Price = array[3].ToObject<decimal>(),
                    Side = array[2].ToObject<decimal>() < 0 ? "sell" : "buy"
                };
                OnTradeMapping?.Invoke(this, item);
                return;
            }
            
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
                OnTradeMapping?.Invoke(this,item);
            }
        }
        #endregion

        #region Handler Для метчинга данных
        /// <summary>
        /// Обработчик для матчинга данных
        /// </summary>
        /// <param name="array">Массив данных пришедший с внешнего апи</param>
        public void HandleCandle(JArray array)
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
                OnCandleMapping?.Invoke(this,item);
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
                OnCandleMapping?.Invoke(this,item);
            }
        }
        #endregion


    }
}
