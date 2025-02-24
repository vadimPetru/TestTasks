namespace TestTask.Service.Cients.Interfaces;

internal interface IClient
{
    public Task<IEnumerable<T>> GetTradesAsync<T>(string symbols);
}
