using Newtonsoft.Json.Linq;
using TestHQ;

namespace TestTask.Service.HandleProcessing.Interface
{
    public interface IHandler
    {
        public event EventHandler<Trade> OnTradeMapping;
        public event EventHandler<Candle> OnCandleMapping;
        void HandleEvent(JToken eventMessage);
        void HandleData(JToken messageData);
        void HandleTrades(JArray array);
        void HandleCandle(JArray array);
    }
}
