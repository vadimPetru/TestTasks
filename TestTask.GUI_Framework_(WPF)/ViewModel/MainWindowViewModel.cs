using ConnectorTest;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TestHQ;
using TestTask.GUI_Framework__WPF_.Infrastructure.Commands;
using TestTask.GUI_Framework__WPF_.Model;
using TestTask.GUI_Framework__WPF_.ViewModel.Base;
using TestTask.Service.Connector.Implementation;

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
    /// <summary>
    /// Текущая коллекция Websokcet
    /// </summary>
    private object _currentCollectionWebSocket;
    public object CurrentCollectionWebSocket
    {
        get => _currentCollectionWebSocket;
        set => Set(ref _currentCollectionWebSocket, value);
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

    #region Коллекция для WebSocket TradeBuy
    private ObservableCollection<Trade> _buy;
    public ObservableCollection<Trade> Buy
    {
        get => _buy;
        set => Set(ref _buy, value);
    }
    #endregion

    #region Коллекция для WebSocket Candle
    private ObservableCollection<Candle> _candle;
    public ObservableCollection<Candle> Candle
    {
        get => _candle;
        set => Set(ref _candle, value);
    }

    #endregion

    #region Команда для Торгов
    public ICommand FetchTradeDataCommand { get; }

    private bool CanFetchDataCommandExecute(object p) => true;

    private async void OnFetchDataCommandExecuted(object p)
    {

        try
        {
            Trades = new();

            var result = await _connector.GetNewTradesAsync("tBTCUSD", 100);

            if (result.IsFailure)
            {
                throw new Exception("Ошибка при получения данных");
            }

            Trades.Clear();
            foreach (var trade in result.Value)
            {
                Trades.Add(trade);
            }
            CurrentCollection = Trades;

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (result.IsFailure)
            {
                throw new Exception("Ошибка получения данных");
            }
            Candles.Clear();
            foreach (var candle in result.Value)
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
    }
    #endregion

    #region Команды WebSocket

    #region Subscribe Trade
    public ICommand SubscribeTradeCommand { get; }
    private bool CanSubscribeTradeCommandExecute(object p) => true;

    private async void OnSubscribeTradeCommandExecuted(object p)
    {
        try
        {
            CurrentCollectionWebSocket = null;
            Buy = new();
            Buy.Clear();
            await _connector.ConnectAsync();
            await _connector.SubscribeTrades("tBTCUSD", 100);
            _connector.NewBuyTrade += (sender, trade) =>
            {
                Application.Current.Dispatcher.Invoke(() => Buy.Add(trade));
            };

            _connector.NewSellTrade += (sender, trade) =>
            {
                Application.Current.Dispatcher.Invoke(() => Buy.Add(trade));
            };

            CurrentCollectionWebSocket = Buy;
            _connector.Processing();

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            await _connector.UnsubscribeTrades("tBTCUSD");
        }



    }
    #endregion

    #region Unsubscribe Trade
    public ICommand UnSubscribeTradeCommand { get; }

    private bool CanUnSubscribeTradeCommandExecute(object p) => true;

    private async void OnUnSubscribeTradeCommandExecuted(object p)
    {
        try
        {
            await _connector.UnsubscribeTrades("tBTCUSD");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            await _connector.UnsubscribeTrades("tBTCUSD");
        }
    }
    #endregion


    #region Subscribe Candle
    public ICommand SubscribeCandleCommand { get; }
    private bool CanSubscribeCandleCommandExecute(object p) => true;

    private async void OnSubscribeCandleCommandExecuted(object p)
    {
        try
        {

            CurrentCollectionWebSocket = null;
            Candle = new();
            Candle.Clear();

            await _connector.ConnectAsync();
            await _connector.SubscribeCandles("tBTCUSD",
             60,
             DateTimeOffset.UtcNow.AddDays(-1),
             count: 30
                 );

            _connector.CandleSeriesProcessing += candle => Candle.Add(candle);
            CurrentCollectionWebSocket = Candle;
            _connector.Processing();

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            await _connector.UnsubscribeCandles("tBTCUSD");
        }



    }

    #endregion

    #region UnSubscribe Candle
    public ICommand UnSubscribeCandleCommand { get; }

    private bool CanUnSubscribeCandleCommandExecute(object p) => true;

    private async void OnUnSubscribeCandleCommandExecuted(object p)
    {
        try
        {
            await _connector.UnsubscribeCandles("tBTCUSD");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    #endregion

    #endregion

    #region Рассчет кошелька
    private ObservableCollection<Balance> _balance;

    public ObservableCollection<Balance> Balance {
        get => _balance;
        set => Set(ref _balance, value);
    }
    public ICommand CalculateWallet { get; }
    public bool CanCalculatedWalletExecute(object p) => true;
    public void OnCalculatedWalletExecuted(object p)
    {
        if (Balance is null)
            Balance = new();
        Balance.Clear();
        var Rates = new Dictionary<string, decimal>()
        {
          {"BTC"  , 93100m   },
          {"XRP"  , 33289.5m },
          {"XMR"  , 405.51m  },
          {"DASH" , 3486.01m }
         };

        var balances = new ObservableCollection<Balance> {

            new Balance("BTC",1,new List<decimal>()),
            new Balance("XRP",1500,new List<decimal>()),
            new Balance("XMR" , 50 , new List<decimal>()),
            new Balance("DASH", 30 , new List<decimal>())
        };

        foreach(var balance in balances)
        {
            balance.ExchangeRates.Add(balance.Amount * Rates[balance.Name]);
            balance.ExchangeRates.Add(balance.ExchangeRates[0] / Rates["BTC"]);
            balance.ExchangeRates.Add(balance.ExchangeRates[0] / Rates["XRP"]);
            balance.ExchangeRates.Add(balance.ExchangeRates[0] / Rates["XMR"]);
            balance.ExchangeRates.Add(balance.ExchangeRates[0] / Rates["DASH"]);
        }

       
        Balance = balances;

    }
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
        SubscribeTradeCommand = new SubscribeTradeWebSocketCommand(OnSubscribeTradeCommandExecuted, CanSubscribeTradeCommandExecute);
        UnSubscribeTradeCommand = new UnSubscribeTradeWebSocketCommand(OnUnSubscribeTradeCommandExecuted, CanUnSubscribeTradeCommandExecute);
        SubscribeCandleCommand = new SubscribeCandleWebSocketCommand(OnSubscribeCandleCommandExecuted, CanSubscribeCandleCommandExecute);
        UnSubscribeCandleCommand = new UnSubscribeCandleWebSocketCommand(OnUnSubscribeCandleCommandExecuted, CanUnSubscribeCandleCommandExecute);
        CalculateWallet = new CalculateWalletCommand(OnCalculatedWalletExecuted, CanCalculatedWalletExecute);
        #endregion

        _connector = connector;
    }
}
