namespace TestTask.Models.Models;

public sealed class Subscription
{
    public int ChanId { get; private set; }

    public string Channel { get; private set; }

    public int MaxCount { get; private set; }


    public void ChangeModel(string channel, int chanId = default)
    {
        if (!string.IsNullOrWhiteSpace(channel))
            Channel = channel;

        if(chanId >0)
           ChanId = chanId;
    }

    private Subscription(int chanId,
        string channel,
        int maxCount
        )
    {
        ChanId = chanId;
        Channel = channel;
        MaxCount = maxCount;
    }

    public static Subscription CreateInstance(int chanId,
        string channel,
        int maxCount)
    {
        return new Subscription(chanId,
                 channel,
                 maxCount
         );
    }

}

public sealed record SubscriptionKey(string symbol, 
    string channel);

