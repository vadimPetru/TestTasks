using TestTask.GUI_Framework__WPF_.ViewModel.Base;

namespace TestTask.GUI_Framework__WPF_.ViewModel.Modal;

internal class CandleContentViewModel : ViewModelBase
{
    #region Пара для ввода

    private string _pair;
    public string Pair { get => _pair; set => Set(ref _pair, value); }
    #endregion

    #region Дата для ввода
    private DateTimeOffset _date;
    public DateTimeOffset Date { get => _date; set => Set(ref _date, value); }
    #endregion

    #region Элемент для выбора
    private string _element;

    public string Element { get => _element; set => Set(ref _element,value); }
    #endregion

    #region Количество для ввода
    private string _count;

    public string Count { get => _count; set => Set(ref _count, value); }
    #endregion


    public CandleContentViewModel()
    {
        
    }
}
