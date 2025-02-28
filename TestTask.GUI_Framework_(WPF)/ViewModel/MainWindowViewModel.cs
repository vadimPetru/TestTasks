using ConnectorTest;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TestHQ;
using TestTask.GUI_Framework__WPF_.Infrastructure.Commands;
using TestTask.GUI_Framework__WPF_.ViewModel.Base;

namespace TestTask.GUI_Framework__WPF_.ViewModel;

internal class MainWindowViewModel : ViewModelBase
{
    #region Название окна
    private string _title = "Тестовое задание";
    /// <summary>
    /// Заголовок окна
    /// </summary>
    public string Title {
        get => _title;
        set => Set(ref _title, value);
    }
    #endregion

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

    #region Коллекции Trade

    private ObservableCollection<Trade> _trades;
    public ObservableCollection<Trade> Trades
    {
        get => _trades;
        set => Set(ref _trades, value);
    }
    
    #endregion

    #region Команды Rest

    #region Команда для Торгов
    public ICommand FetchTradeDataCommand { get; }

    private bool CanFetchDataCommandExecute(object p) => true;

    private async void OnFetchDataCommandExecuted(object p)
    {
        
        try
        {
            
            var result = await _connector.GetNewTradesAsync("tBTCUSD", 100);

            Trades.Clear();
            foreach(var trade in result)
            {
                Trades.Add(trade);
            }
            

            }catch(Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
       
    }
    #endregion

    #region Команда для Свечей
    public  ICommand FetchCandleDataCommand { get; }

    private bool CanFetchCandleDataCommandExecute(object p) => true;

    private async void OnFetchCandleDataCommandExecuted(object p)
    {
        try
        {
            var result = await _connector.GetCandleSeriesAsync("tBTCUSD",
                60,
                DateTimeOffset.UtcNow.AddDays(-1),
                count: 30);
        }catch(Exception ex)
        {

        }
    }
    #endregion

    #endregion

    private readonly ITestConnector _connector;
    public MainWindowViewModel()
    {
        
    }
    public MainWindowViewModel(ITestConnector connector)
    {
        #region Инициализация комманды

        FetchTradeDataCommand = new TradeRestCommand(OnFetchDataCommandExecuted, CanFetchDataCommandExecute);
        FetchCandleDataCommand = new CandleRestCommand(OnFetchCandleDataCommandExecuted, CanFetchCandleDataCommandExecute);

        #endregion

        _connector = connector;

        Trades = new();
    }
}
