using System.Windows.Input;
using TestTask.GUI_Framework__WPF_.Infrastructure.Commands;
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
        private object _dialogContent;
        public object DialogContent
        {
            get => _dialogContent;
            set => Set(ref _dialogContent, value);
        }
        #endregion

        #region Команды 
        public ICommand OkCommand { get; }

        private bool CanOkCommandExecute(object p) => true;

        public  void OnOkCommandExecuted(object p) =>  OnDialogClosed?.Invoke(this, true);
       

        public ICommand CancelCommand { get; }

        private bool CanCancelCommandExecute(object p) => true;

        public void OnCancelCommandExecuted(object p) => OnDialogClosed?.Invoke(this, false);
        #endregion

        public event EventHandler<bool> OnDialogClosed;
        public ModelWindowViewModel()
        {
            OkCommand = new OkCommand(OnOkCommandExecuted, CanOkCommandExecute);
            CancelCommand = new CancelCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
        }
    }
}
