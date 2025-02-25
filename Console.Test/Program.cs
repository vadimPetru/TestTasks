using Microsoft.Extensions.DependencyInjection;
using TestTask.Service.Cients.Implementation;
using TestTask.Service.Cients.Interfaces;
using TestTask.Service.Connector.Implementation;
using ConnectorTest;

var services = new ServiceCollection();

services.AddHttpClient();

services.AddSingleton<IClient>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new RestClient(httpClientFactory);
});


services.AddSingleton<Connector>();

var serviceProvider = services.BuildServiceProvider();
var connector = serviceProvider.GetRequiredService<Connector>();

try
{
    var orderBook = await connector.GetNewTradesAsync("tBTCUSD");
    Console.WriteLine(orderBook.First().Id + orderBook.Last().Id);
}catch(Exception ex)
{
    throw new Exception(ex.Message);
}