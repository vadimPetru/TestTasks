using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using TestTask.Service.Cients.Interfaces;

namespace TestTask.Service.Cients.Implementation
{
    public class WebSocketClient : IWebSocketClient, IDisposable
    {
        private readonly ClientWebSocket _webSocket;
        private readonly Uri _serverUri;
        private readonly CancellationTokenSource _ctx;

        public event EventHandler<string> OnMessageReceived;
        public event EventHandler<string> OnError;

        public WebSocketClient(string serverUrl)
        {
            _webSocket = new ClientWebSocket();
            _serverUri = new Uri(serverUrl);
            _ctx = new CancellationTokenSource();
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _webSocket.ConnectAsync(_serverUri, _ctx.Token);
                await ListenForMessageAsync();
            }
            catch(WebSocketException ex)
            {
                OnError?.Invoke(this, ex.Message);
            }catch(Exception ex)
            {
                OnError?.Invoke(this, ex.Message);
            }
        }
        public async Task SendEventAsync(string eventName , object data)
        {
            if(_webSocket.State != WebSocketState.Open)
            {
                OnError?.Invoke(this, "WebSocket is not open");
            }

            try
            {
                var eventObject = new {
                    Event = eventName,
                    Data = data
                };

                string jsonMessage = JsonConvert.SerializeObject(eventObject);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _ctx.Token);
            }
            catch (WebSocketException ex)
            {
                OnError?.Invoke(this, ex.Message);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex.Message);
            }
        }
        private async Task ListenForMessageAsync()
        {
            while(_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                        _ctx.Token);
                    var receiveMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    OnMessageReceived?.Invoke(this, receiveMessage);

                }catch(WebSocketException ex)
                {
                    OnError?.Invoke(this, ex.Message);
                    break;
                }
            }
        }

        public async Task CloseAsync()
        {
            if(_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseReceived)
            {
                try
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", _ctx.Token);
                }
                catch(WebSocketException ex)
                {
                    OnError?.Invoke(this, ex.Message);
                }catch(Exception ex)
                {
                    OnError?.Invoke(this, ex.Message);
                }
            }
        }
        public void Dispose()
        {
            _webSocket?.Dispose();
            _ctx?.Dispose();
        }
    }
}
