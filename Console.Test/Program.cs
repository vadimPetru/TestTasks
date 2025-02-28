using Microsoft.Extensions.DependencyInjection;
using TestTask.Service.Cients.Implementation;
using TestTask.Service.Cients.Interfaces;
using TestTask.Service.Connector.Implementation;

var services = new ServiceCollection();

services.AddHttpClient();

services.AddSingleton<IRestClient>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new RestClient(httpClientFactory);
});


services.AddSingleton<Connector>();

var serviceProvider = services.BuildServiceProvider();
var connector = serviceProvider.GetRequiredService<Connector>();

try
{
    var trades = await connector.GetNewTradesAsync("tBTCUSD");
    var candels = await connector.GetCandleSeriesAsync("tBTCUSD",
        60,
        DateTimeOffset.UtcNow.AddDays(-1),
        count: 30
        );

    Console.WriteLine(trades.First().Id + trades.Last().Id); 
}catch(Exception ex)
{
    throw new Exception(ex.Message);
}


await connector.ConnectAsync();
connector.NewBuyTrade += trade => Console.WriteLine($"New buy trade: {trade}");
connector.NewSellTrade += trade => Console.WriteLine($"New sell trade: {trade}");
await connector.SubscribeTrades("tBTCUSD");
await connector.SubscribeCandles("tBTCUSD",
    60,
    DateTimeOffset.UtcNow.AddDays(-1),
    count: 30
    );
connector.Processing();
await Task.Delay(25000); // Ждем 25 секунд
await connector.UnsubscribeTrades("tBTCUSD");
Console.ReadKey();


