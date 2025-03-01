using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;
using System.Text;
using TestHQ;
using TestTask.Service.Cients.Interfaces;

namespace TestTask.Service.Cients.Implementation
{
    public class WebSocketClient : IWebSocketClient
    {
        private readonly ClientWebSocket _webSocket;
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
            try
            {
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
                            "Соединение закрыто",
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
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", _ctx.Token);
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


        public void HandleEvent(JToken eventMessage)
        {
            string eventType = eventMessage["event"]?.ToString()!;
            string chanId = eventMessage["chanId"]?.ToString()!;

            switch (eventType)
            {
                case "info":
                    OnInfoMessage?.Invoke(this, eventMessage);
                    break;
                case "subscribed":
                    OnSubscribeMessage?.Invoke(this, eventMessage);
                    break;
                case "unsubscribed":
                    OnSubscribeMessage?.Invoke(this, eventMessage);
                    break;
                default:
                    OnError?.Invoke(this, eventType);
                    break;
            }
        }
        public void HandleData(JToken messageData)
        {
            var channelId = messageData[0].ToObject<int>();
            var array = messageData[1] as JArray;

            if (array is null || array.Count == 0)
            {
                OnError.Invoke(this, "Пустой массив");
                return;
            }

            if (array.Count == 6)
            {
                var candle = new Candle();
                candle.HandleCandle(array);


                return;
            }
            var firstItem = array[0] as JArray;

            if (firstItem.Count == 4)
            {
                new Trade().HandleTrades(array);
                return;
            }
            else
            {
                new Candle().HandleCandle(array);
                return;
            }
        }

        public void Dispose()
        {
            _webSocket?.Dispose();
            _ctx?.Dispose();
        }
    }
}
