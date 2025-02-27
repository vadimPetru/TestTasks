using TestTask.GUI_Framework__WPF_.ViewModel.Base;

namespace TestTask.GUI_Framework__WPF_.ViewModel;

internal class MainWindowViewModel : ViewModelBase
{

    private string _title = "Тестовое задание";
    /// <summary>
    /// Заголовок окна
    /// </summary>
    public string Title {
        get => _title;
        set => Set(ref _title, value);
    }
    #region Модели для Окна Rest
    private string _tradeButton = "Торги";
    /// <summary>
    /// Получение Торгов по Rest
    /// </summary>
    public string TradeButton
    {
        get => _tradeButton;
        set => Set(ref _tradeButton, value);
    }

    private string _candleButton = "Свечи";
    /// <summary>
    /// Получение Cвечей по Rest
    /// </summary>
    public string CandleButton
    {
        get => _candleButton;
        set => Set(ref _candleButton, value);
    }

    private string _tickerButton = "Тикеры";
    /// <summary>
    /// Получение Тикеров по Rest
    /// </summary>
    public string TickerButton
    {
        get => _tickerButton;
        set => Set(ref _tickerButton, value);
    }
    #endregion
}
