﻿using TestHQ;

namespace ConnectorTest;

interface ITestConnector
{
    #region Rest

    Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount);
    Task<IEnumerable<Candle>> GetCandleSeriesAsync(string pair, int periodInSec, DateTimeOffset? from, DateTimeOffset? to = null, long? count = 0);

    #endregion

    #region Socket


    event Action<Trade> NewBuyTrade;
    event Action<Trade> NewSellTrade;
    Task SubscribeTrades(string pair, int maxCount = 100);
    Task UnsubscribeTrades(string pair);



    event Action<Candle> CandleSeriesProcessing;
    Task SubscribeCandles(string pair,
        int periodInSec,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null, long? count = 0
        );
    //Task UnsubscribeCandles(string pair);

    #endregion

}
