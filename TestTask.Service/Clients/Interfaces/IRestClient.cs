using TestHQ;
using TestTask.Models.Models;

namespace TestTask.Service.Cients.Interfaces;

public interface IRestClient
{
    public Task<List<Trade>> GetTradesAsync(string symbols);
    public Task<List<Candle>> GetCandleAsync(string pair,
            string candle,
            long startTime,
            long endTime,
            long? count = 0);
    public Task<Ticker> GetTickerAsync(string symbol); 
}
