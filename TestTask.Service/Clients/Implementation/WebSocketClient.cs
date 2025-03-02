using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using TestHQ;
using TestTask.Models.Enums;
using TestTask.Service.Cients.Interfaces;

namespace TestTask.Service.Cients.Implementation
{
    public class WebSocketClient : IWebSocketClient
    {
        private ClientWebSocket _webSocket;
        private readonly Uri _serverUri;
        private readonly CancellationTokenSource _ctx;

        public event EventHandler<string> OnMessageReceived;
        public event EventHandler<string> OnError;
        public event EventHandler<object> OnInfoMessage;
        public event EventHandler<object> OnSubscribeMessage;
        public event EventHandler<object> OnUnSubscribeMessage;

        public WebSocketClient(string serverUrl)
        {
            _webSocket = new ClientWebSocket();
            _serverUri = new Uri(serverUrl);
            _ctx = new CancellationTokenSource();
            
        }

        public async Task ConnectAsync()
        {
           
            if (_webSocket.State == WebSocketState.Open)
            {
                return; 
            }

            if (_webSocket == null || _webSocket.State == WebSocketState.Closed || _webSocket.State == WebSocketState.Aborted)
            {
                _webSocket?.Dispose(); 
                _webSocket = new ClientWebSocket(); 
            }
            try
                {
                    // Создаем новое соединение
                    await _webSocket.ConnectAsync(_serverUri, _ctx.Token);
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
        public async Task SendEventAsync(object message)
        {
            if (_webSocket.State != WebSocketState.Open)
            {
                OnError?.Invoke(this, "WebSocket is not open");
            }

            try
            {
                string jsonMessage = JsonConvert.SerializeObject(message);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text, true, _ctx.Token
                 );
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
        public async Task ListenForMessageAsync()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 100];
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer),
                        _ctx.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                            "close",
                            _ctx.Token
                            );
                    }
                    var receiveMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    OnMessageReceived?.Invoke(this, receiveMessage);

                }
                catch (WebSocketException ex)
                {
                    OnError?.Invoke(this, ex.Message);
                    break;
                }

            }
        }
        public bool IsHeartbeat(JToken message)
        {
            return (message is JArray array && array.Count == 0) ||
            (message is JArray hbArray
            && hbArray.Count == 2 && hbArray[1].ToString() == "hb");
        }
        public async Task CloseAsync()
        {
            if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseReceived)
            {
                try
                {
               
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "close", _ctx.Token);
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
        }
        public void Dispose()
        {
            _webSocket?.Dispose();
            _ctx?.Dispose();
        }
    }
}
