using TestTask.GUI_Framework__WPF_.Infrastructure.Commands.CommandsBase;

namespace TestTask.GUI_Framework__WPF_.Infrastructure.Commands;

internal class SubscribeTradeWebSocketCommand : CommandBase
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool> _canExecute;

    public SubscribeTradeWebSocketCommand(Action<object> execute, Func<object, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public override bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;


    public override void Execute(object? parameter) => _execute(parameter);
}
