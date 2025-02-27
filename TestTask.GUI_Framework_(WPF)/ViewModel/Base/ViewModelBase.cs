using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TestTask.GUI_Framework__WPF_.ViewModel.Base;


/// <summary>
/// Базовый класс для всех ViewModel ,позволяет уведомлять UI об изменениях
/// </summary>
internal abstract class ViewModelBase : INotifyPropertyChanged
{
    private bool _Disposed;
    public event PropertyChangedEventHandler? PropertyChanged;


    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
    {
        if (Equals(field, value)) return false;

        field = value;
        OnPropertyChanged(PropertyName);
        return true;
    }
}
