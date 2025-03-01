namespace TestTask.Models.Models.Event;

public class TradeUpdatedEventArgs
{
    public TradeUpdatedEventArgs(int channelId, object tradeData)
    {
        ChannelId = channelId;
        TradeData = tradeData;
    }

    public int ChannelId { get; }
    public object TradeData { get; }
}
