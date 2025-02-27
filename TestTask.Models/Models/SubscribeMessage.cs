namespace TestTask.Models.Models
{
    /// <summary>
    /// Сообщение для подписки на Trade
    /// </summary>
    /// <param name="event">Название события</param>
    /// <param name="channel">Название канала</param>
    /// <param name="symbol">Символ валюты</param>
    public record SubscribeTradeMessage(string @event,
        string channel,
        string symbol
    );

    /// <summary>
    /// Сообщение для подписки на Candle
    /// </summary>
    /// <param name="event">Название события</param>
    /// <param name="channel">Название канала</param>
    /// <param name="key">Символ валюты</param>
    public record SubscribeCandleMessage(string @event,
        string channel,
        string key
    );

    /// <summary>
    /// Сообщение для отписки на Trade
    /// </summary>
    /// <param name="event">Название события</param>
    /// <param name="chatId">Индификатор чата для отписки</param>
    public record UnSubscribeTradeMessage(string @event,
        int chanId
    );

    /// <summary>
    /// Сообщение для отписки на Candle
    /// </summary>
    /// <param name="event">Название события</param>
    /// <param name="chatId">Индификатор чата для отписки</param>
    public record UnSubscribeCandleMessage(string @event,
        int chatId
    );
}
