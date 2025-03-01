using System.Runtime.CompilerServices;
using System.Windows.Input;
using TestHQ;
using TestTask.GUI_Framework__WPF_.Infrastructure.Commands;
using TestTask.GUI_Framework__WPF_.View.Modal;
using TestTask.GUI_Framework__WPF_.ViewModel.Base;

namespace TestTask.GUI_Framework__WPF_.ViewModel
{
    internal class ModelWindowViewModel : ViewModelBase
    {
        #region Заголовок окна
        private string _title;
        public string Title { get => _title;
            set => Set(ref _title, value); 
        }
        #endregion

        #region Динамический контент
        private IModal _dialogContent;
        public IModal DialogContent
        {
            get => _dialogContent;
            set => Set(ref _dialogContent, value);
        }
        #endregion

        #region Команды Отправки
        public ICommand OkCommand { get; }

        private bool CanOkCommandExecute(object p) => true;

        public  void OnOkCommandExecuted(object p) =>  OnDialogClosed?.Invoke(this, true);

        #endregion

        #region Команда Отмены
        public ICommand CancelCommand { get; }

        private bool CanCancelCommandExecute(object p) => true;

        public void OnCancelCommandExecuted(object p) => OnDialogClosed?.Invoke(this, false);
        #endregion

        public void LoadDynamicContent(string ContentName)
        {
            if(ContentName == "Candle")
                DialogContent = new CandleContent();
            DialogContent = new TradeContent();
        }


     
        public event EventHandler<bool> OnDialogClosed;
        public ModelWindowViewModel()
        {
            OkCommand = new OkCommand(OnOkCommandExecuted, CanOkCommandExecute);
            CancelCommand = new CancelCommand(OnCancelCommandExecuted, CanCancelCommandExecute);

            LoadDynamicContent("Candle");
        }
    }
}
