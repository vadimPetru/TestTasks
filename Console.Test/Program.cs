using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using TestHQ;
using TestTask.Models.Models.Event;
using TestTask.Service.Cients.Implementation;
using TestTask.Service.Cients.Interfaces;
using TestTask.Service.Connector.Implementation;
using TestTask.Service.HandleProcessing.Implementation;
using TestTask.Service.HandleProcessing.Interface;

var services = new ServiceCollection();

services.AddHttpClient();

services.AddSingleton<IRestClient>(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
    return new RestClient(httpClientFactory);
});

services.AddSingleton<IHandler, Handler>();
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

    Console.WriteLine(trades.Value.First().Id + trades.Value.Last().Id); 
}catch(Exception ex)
{
    throw new Exception(ex.Message);
}


await connector.ConnectAsync();
connector.NewBuyTrade += (sender , trade) => Console.WriteLine($"New buy trade: {trade.Id}");
connector.NewSellTrade += (sender ,trade )=> Console.WriteLine($"New sell trade: {trade.Id}");
await connector.SubscribeTrades("tBTCUSD");
await connector.SubscribeCandles("tBTCUSD",
60,
    DateTimeOffset.UtcNow.AddDays(-1),
    count: 30
    );
connector.TradeExecuted += TradeExecuted;
connector.TradeUpdated += TradeUpdated;
connector.Processing();
await Task.Delay(150000); // Ждем 25 секунд
await connector.UnsubscribeTrades("tBTCUSD");
await connector.UnsubscribeCandles("tBTCUSD");
Console.ReadKey();


void TradeExecuted(TradeExecutedEventArgs message)
{
    Console.WriteLine(message.ChannelId);
}
void TradeUpdated(TradeUpdatedEventArgs message)
{
    Console.WriteLine(message.ChannelId);
}
