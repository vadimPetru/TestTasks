namespace TestTask.Models.Models;
/// <summary>
///  Ticket
/// </summary>
/// <param name="Bid">Цена последней наивысшей ставки</param>
/// <param name="BidSize">Сумма 25 самых высоких ставок</param>
/// <param name="Ask">Цена последнего самого никзкого запроса</param>
/// <param name="AskSize">Сумма 25 самых низких запрашиваемых размеров</param>
/// <param name="DailyChange">Сумма, на которую изменилась последняя цена со вчерашнего дня</param>
/// <param name="DailyChangeRelative">Относительное изменение цены со вчерашнего дня</param>
/// <param name="LastPrice">Цена последней сделки</param>
/// <param name="Volume">Ежедневный обьем</param>
/// <param name="High">Дневной максимум</param>
/// <param name="Low">Дневной минимум</param>
public sealed record Ticker(decimal Bid,
    decimal BidSize,
    decimal Ask,
    decimal AskSize,
    decimal DailyChange,
    decimal DailyChangeRelative,
    decimal LastPrice,
    decimal Volume,
    decimal High,
    decimal Low);
