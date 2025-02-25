using TestHQ;
using TestTask.Models.Models;
using TestTask.Service.Cients.Interfaces;

namespace TestTask.Service.Connector.Implementation;

public class Connector(IRestClient client)
{
    private readonly IRestClient _client = client;

    public async Task<IEnumerable<Trade>> GetNewTradesAsync(string pair, int maxCount = 0)
    {
        try
        {
            var result = await _client.GetTradesAsync(pair);
            return result;

        }catch(Exception Ex)
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

        }catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
