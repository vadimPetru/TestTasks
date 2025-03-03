using ConnectorTest;
using Moq;
using TestHQ;
using TestTask.Models.Models;
using TestTask.Models.ResultObject;
using TestTask.Service.Cients.Interfaces;
using TestTask.Service.Connector.Implementation;
using TestTask.Service.HandleProcessing.Interface;

namespace TestTask.IntegrationTest.Rest;

public class ConnectorTestClient
{
    private readonly Mock<IRestClient> MockRestClient;
    private readonly Mock<IHandler> MockHandler;
    private readonly ITestConnector _connector;

    public ConnectorTestClient()
    {
        MockRestClient = new Mock<IRestClient>();
        MockHandler = new Mock<IHandler>();
        _connector = new Connector(MockRestClient.Object, MockHandler.Object);
    }


    [Fact]
    public async Task GetTradeAsync_ShouldReturnTrades_Success()
    {
        //Arrange 
        var pair = "tBTCUSD";
        var expectedTrades = new List<Trade>
        {
            new Trade { Id="1" ,Pair = pair, Price = 50000, Amount=2 ,Side= "sell"},
            new Trade { Id="2" ,Pair = pair, Price = 51000, Amount=2 ,Side= "buy"}
        };

        MockRestClient.Setup(client => client.GetTradesAsync(pair))
            .ReturnsAsync(expectedTrades);

        //Act 
        var result = await _connector.GetNewTradesAsync(pair, 100);


        //Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTrades, result.Value);
    }

    [Fact]
    public async Task GetTradeAsync_ShouldReturnFailure_Failure()
    {
        //Arrange 

        var pair = "tBTCUSD";
        var exeptionMessage = "Network error";

        MockRestClient.Setup(client => client.GetTradesAsync(pair))
            .Throws(new Exception(exeptionMessage));

        //Act 
        var result = await _connector.GetNewTradesAsync(pair, 100);

        //Assert
        Assert.True(result.IsFailure);
        Assert.Equal("01", result.Error.Code);
        Assert.Equal(exeptionMessage, result.Error.Message);
    }

    

    public 
}
