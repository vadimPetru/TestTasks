using TestHQ;
using TestTask.Models.Models;
using TestTask.Models.ResultObject;

namespace ConnectorTest;

public interface ITestConnector
{
    #region Rest

    Task<Result<IEnumerable<Trade>>> GetNewTradesAsync(string pair, int maxCount);
    Task<Result<IEnumerable<Candle>>> GetCandleSeriesAsync(string pair,
        int periodInSec,
        DateTimeOffset? from,
        DateTimeOffset? to = null,
        long? count = 0);
    Task<Result<Ticker>> GetTicker(string symbol);
    #endregion

    #region Socket


    event EventHandler<Trade> NewBuyTrade;
    event EventHandler<Trade> NewSellTrade;
    Task SubscribeTrades(string pair, int maxCount = 100);
    Task UnsubscribeTrades(string pair);
    Task ConnectAsync();
    Task Processing();

    event Action<Candle> CandleSeriesProcessing;
    Task SubscribeCandles(string pair,
        int periodInSec,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null, long? count = 0
        );
    Task UnsubscribeCandles(string pair);

    #endregion

}
