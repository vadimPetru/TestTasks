namespace TestTask.Models.Models.Event
{
    public sealed class TradeExecutedEventArgs : EventArgs
    {
        public TradeExecutedEventArgs(int channelId, object tradeData)
        {
            ChannelId = channelId;
            TradeData = tradeData;
        }

        public int ChannelId { get; }
        public object TradeData { get; }
    }
}
