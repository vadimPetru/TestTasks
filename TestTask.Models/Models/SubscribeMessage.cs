namespace TestTask.Models.Models
{
    /// <summary>
    /// Сообщение для подписки на торги
    /// </summary>
    /// <param name="event">Название события</param>
    /// <param name="channel">Название канала</param>
    /// <param name="symbol">Символ валюты</param>
    public record SubscribeTradeMessage(string @event,
        string channel,
        string symbol
    );

    /// <summary>
    /// Сообщение для подписки Свечи
    /// </summary>
    /// <param name="event">Название события</param>
    /// <param name="channel">Название канала</param>
    /// <param name="key">Символ валюты</param>
    public record SubscribeCandleMessage(string @event,
        string channel,
        string key
    );

    /// <summary>
    /// Сообщение отписки
    /// </summary>
    /// <param name="event">Название события</param>
    public record UnSubscribeTradeMessage(string @event
    );

}
