using TestTask.GUI_Framework__WPF_.ViewModel.Base;

namespace TestTask.GUI_Framework__WPF_.ViewModel.Modal;

internal class TradeContentViewModel : ViewModelBase
{
    #region Пара для ввода

    private string _pair;
    public string Pair { get => _pair; set => Set(ref _pair, value); }
    #endregion
}
