using TestHQ;
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
        long? count = 0)
    {
        try
        {
            var result = await _client.GetCandleAsync();
        }catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
