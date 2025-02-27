using System.Windows.Input;

namespace TestTask.GUI_Framework__WPF_.Infrastructure.Commands.CommandBase
{
    internal abstract class CommandBase : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public abstract bool CanExecute(object? parameter);


        public abstract void Execute(object? parameter);
       
    }
}
