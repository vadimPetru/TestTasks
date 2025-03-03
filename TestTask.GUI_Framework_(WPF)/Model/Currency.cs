namespace TestTask.GUI_Framework__WPF_.Model
{
    public sealed record Balance(string Name,
        decimal Amount,
        List<decimal> ExchangeRates
    );
}
