using TestHQ;

namespace TestTask.Service.Cients.Interfaces;

public interface IClient
{
    public Task<List<Trade>> GetTradesAsync(string symbols);
}
