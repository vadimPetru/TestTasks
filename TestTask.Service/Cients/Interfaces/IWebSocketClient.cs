using TestTask.Models.Models;

namespace TestTask.Service.Cients.Interfaces;

public interface IWebSocketClient : IDisposable
{
    event EventHandler<string> OnMessageReceived;
    event EventHandler<string> OnError;
    Task ListenForMessageAsync();
    Task ConnectAsync();
    Task SendEventAsync(object message);
    Task CloseAsync();
}
