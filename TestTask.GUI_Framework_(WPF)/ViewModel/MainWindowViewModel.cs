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
    public string Title
    {
        get => _title;
        set => Set(ref _title, value);
    }
    #endregion

    #region Модели для Окна Rest

    /// <summary>
    /// Получение Торгов по Rest
    /// </summary>
    private string _tradeButton = "Торги";
    public string TradeButton
    {
        get => _tradeButton;
        set => Set(ref _tradeButton, value);
    }


    /// <summary>
    /// Получение Cвечей по Rest
    /// </summary>
    private string _candleButton = "Свечи";
    public string CandleButton
    {
        get => _candleButton;
        set => Set(ref _candleButton, value);
    }


    /// <summary>
    /// Получение Тикеров по Rest
    /// </summary>
    private string _tickerButton = "Тикеры";
    public string TickerButton
    {
        get => _tickerButton;
        set => Set(ref _tickerButton, value);
    }

    /// <summary>
    /// Текущая коллекция
    /// </summary>
    public object _currentCollection;
    public object CurrentCollection
    {
        get => _currentCollection;
        set => Set(ref _currentCollection, value);
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

    #region Коллекция Candle
    private ObservableCollection<Candle> _candles;

    public ObservableCollection<Candle> Candles
    {
        get => _candles;
        set => Set(ref _candles, value);
    }
    #endregion

    #region Видимость окна

    private bool _isModalVisible;
    public bool IsModalVisible
    {
        get => _isModalVisible;
        set => Set(ref _isModalVisible, value);
    }
    #endregion

    #region Команды Rest

    #region Команда для Торгов
    public ICommand ShowTradesCommand { get;}
    public ICommand FetchTradeDataCommand { get; }

    private bool CanFetchDataCommandExecute(object p) => true;

    private async void OnFetchDataCommandExecuted(object p)
    {

        try
        {

            Trades = new();
            var result = await _connector.GetNewTradesAsync("tBTCUSD", 100);

            Trades.Clear();
            foreach (var trade in result)
            {
                Trades.Add(trade);
            }
            CurrentCollection = Trades;

            }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsModalVisible = false;
        }

    }
    #endregion

    #region Команда для Свечей
    public ICommand FetchCandleDataCommand { get; }

    private bool CanFetchCandleDataCommandExecute(object p) => true;

    private async void OnFetchCandleDataCommandExecuted(object p)
    {
        try
        {
            Candles = new();
            var result = await _connector.GetCandleSeriesAsync("tBTCUSD",
                60,
                DateTimeOffset.UtcNow.AddDays(-1),
                count: 30);
            Candles.Clear();
            foreach(var candle in result)
            {
                Candles.Add(candle);
            }
            CurrentCollection = null;
            CurrentCollection = Candles;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsModalVisible = false;
        }
    }
    #endregion

    #endregion

    #region Модельное окно

    #endregion

    private readonly ITestConnector _connector;
    private readonly ModelWindowViewModel _model = new();

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
       
      
       
    }
}
