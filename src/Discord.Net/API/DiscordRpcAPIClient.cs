using Discord.API.Gateway;
using Discord.API.Rest;
using Discord.API.Rpc;
using Discord.Net;
using Discord.Net.Converters;
using Discord.Net.Queue;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
    public class DiscordRpcApiClient : IDisposable
    {
        private object _eventLock = new object();
        
        public event Func<string, Task> SentRpcMessage { add { _sentRpcMessageEvent.Add(value); } remove { _sentRpcMessageEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, Task>> _sentRpcMessageEvent = new AsyncEvent<Func<string, Task>>();

        public event Func<string, string, object, string, Task> ReceivedRpcEvent { add { _receivedRpcEvent.Add(value); } remove { _receivedRpcEvent.Remove(value); } }
        private readonly AsyncEvent<Func<string, string, object, string, Task>> _receivedRpcEvent = new AsyncEvent<Func<string, string, object, string, Task>>();
        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();

        private readonly RequestQueue _requestQueue;
        private readonly JsonSerializer _serializer;
        private readonly IWebSocketClient _webSocketClient;
        private readonly SemaphoreSlim _connectionLock;
        private readonly string _clientId;
        private CancellationTokenSource _loginCancelToken, _connectCancelToken;
        private string _authToken;
        private bool _isDisposed;

        public LoginState LoginState { get; private set; }
        public ConnectionState ConnectionState { get; private set; }

        public DiscordRpcApiClient(string clientId, WebSocketProvider webSocketProvider, JsonSerializer serializer = null, RequestQueue requestQueue = null)
        {
            _connectionLock = new SemaphoreSlim(1, 1);
            _clientId = clientId;

            _requestQueue = requestQueue ?? new RequestQueue();

            if (webSocketProvider != null)
            {
                _webSocketClient = webSocketProvider();
                //_gatewayClient.SetHeader("user-agent", DiscordConfig.UserAgent); (Causes issues in .Net 4.6+)
                _webSocketClient.BinaryMessage += async (data, index, count) =>
                {
                    using (var compressed = new MemoryStream(data, index + 2, count - 2))
                    using (var decompressed = new MemoryStream())
                    {
                        using (var zlib = new DeflateStream(compressed, CompressionMode.Decompress))
                            zlib.CopyTo(decompressed);
                        decompressed.Position = 0;
                        using (var reader = new StreamReader(decompressed))
                        {
                            var msg = JsonConvert.DeserializeObject<RpcMessage>(reader.ReadToEnd());
                            await _receivedRpcEvent.InvokeAsync(msg.Cmd, msg.Event, msg.Data, msg.Nonce).ConfigureAwait(false);
                        }
                    }
                };
                _webSocketClient.TextMessage += async text =>
                {
                    var msg = JsonConvert.DeserializeObject<RpcMessage>(text);
                    await _receivedRpcEvent.InvokeAsync(msg.Cmd, msg.Event, msg.Data, msg.Nonce).ConfigureAwait(false);
                };
                _webSocketClient.Closed += async ex =>
                {
                    await DisconnectAsync().ConfigureAwait(false);
                    await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
                };
            }

            _serializer = serializer ?? new JsonSerializer { ContractResolver = new DiscordContractResolver() };
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _connectCancelToken?.Dispose();
                    (_webSocketClient as IDisposable)?.Dispose();
                }
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);
        
        public async Task LoginAsync(TokenType tokenType, string token, RequestOptions options = null)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternalAsync(tokenType, token, options).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternalAsync(TokenType tokenType, string token, RequestOptions options = null)
        {
            if (LoginState != LoginState.LoggedOut)
                await LogoutInternalAsync().ConfigureAwait(false);

            if (tokenType != TokenType.Bearer)
                throw new InvalidOperationException("RPC only supports bearer tokens");

            LoginState = LoginState.LoggingIn;
            try
            {
                _loginCancelToken = new CancellationTokenSource();                
                await _requestQueue.SetCancelTokenAsync(_loginCancelToken.Token).ConfigureAwait(false);
                
                _authToken = token;

                LoginState = LoginState.LoggedIn;
            }
            catch (Exception)
            {
                await LogoutInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task LogoutAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LogoutInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LogoutInternalAsync()
        {
            //An exception here will lock the client into the unusable LoggingOut state, but that's probably fine since our client is in an undefined state too.
            if (LoginState == LoginState.LoggedOut) return;
            LoginState = LoginState.LoggingOut;

            try { _loginCancelToken?.Cancel(false); }
            catch { }

            await DisconnectInternalAsync().ConfigureAwait(false);
            await _requestQueue.ClearAsync().ConfigureAwait(false);

            await _requestQueue.SetCancelTokenAsync(CancellationToken.None).ConfigureAwait(false);

            LoginState = LoginState.LoggedOut;
        }

        public async Task ConnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await ConnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync()
        {
            /*if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("You must log in before connecting.");*/

            ConnectionState = ConnectionState.Connecting;
            try
            {
                _connectCancelToken = new CancellationTokenSource();
                if (_webSocketClient != null)
                    _webSocketClient.SetCancelToken(_connectCancelToken.Token);

                bool success = false;
                for (int port = DiscordRpcConfig.PortRangeStart; port <= DiscordRpcConfig.PortRangeEnd; port++)
                {
                    try
                    {
                        string url = $"wss://discordapp.io:{port}/?v={DiscordRpcConfig.RpcAPIVersion}&client_id={_clientId}";
                        await _webSocketClient.ConnectAsync(url).ConfigureAwait(false);
                        success = true;
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }
                if (!success)
                    throw new Exception("Unable to connect to the RPC server.");

                ConnectionState = ConnectionState.Connected;
            }
            catch (Exception)
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync()
        {
            if (_webSocketClient == null)
                throw new NotSupportedException("This client is not configured with websocket support.");

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            
            try { _connectCancelToken?.Cancel(false); }
            catch { }

            await _webSocketClient.DisconnectAsync().ConfigureAwait(false);

            ConnectionState = ConnectionState.Disconnected;
        }

        //Core
        public Task SendRpcAsync(string cmd, object payload, GlobalBucket bucket = GlobalBucket.GeneralRpc, RequestOptions options = null)
            => SendRpcAsyncInternal(cmd, payload, BucketGroup.Global, (int)bucket, 0, options);
        public Task SendRpcAsync(string cmd, object payload, GuildBucket bucket, ulong guildId, RequestOptions options = null)
            => SendRpcAsyncInternal(cmd, payload, BucketGroup.Guild, (int)bucket, guildId, options);
        private async Task SendRpcAsyncInternal(string cmd, object payload, 
            BucketGroup group, int bucketId, ulong guildId, RequestOptions options)
        {
            //TODO: Add Nonce to pair sent requests with responses
            byte[] bytes = null;
            payload = new RpcMessage { Cmd = cmd, Args = payload, Nonce = Guid.NewGuid().ToString() };
            if (payload != null)
                bytes = Encoding.UTF8.GetBytes(SerializeJson(payload));
            await _requestQueue.SendAsync(new WebSocketRequest(_webSocketClient, bytes, true, options), group, bucketId, guildId).ConfigureAwait(false);
            await _sentRpcMessageEvent.InvokeAsync(cmd).ConfigureAwait(false);
        }

        //Rpc
        public async Task SendAuthenticateAsync(RequestOptions options = null)
        {
            var msg = new AuthenticateParams()
            {
                AccessToken = _authToken
            };
            await SendRpcAsync("AUTHENTICATE", msg, options: options).ConfigureAwait(false);
        }
        public async Task SendAuthorizeAsync(string[] scopes, RequestOptions options = null)
        {
            var msg = new AuthorizeParams()
            {
                ClientId = _clientId,
                Scopes = scopes
            };
            await SendRpcAsync("AUTHORIZE", msg, options: options).ConfigureAwait(false);
        }

        //Helpers
        private static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
        private string SerializeJson(object value)
        {
            var sb = new StringBuilder(256);
            using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (JsonWriter writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, value);
            return sb.ToString();
        }
        private T DeserializeJson<T>(Stream jsonStream)
        {
            using (TextReader text = new StreamReader(jsonStream))
            using (JsonReader reader = new JsonTextReader(text))
                return _serializer.Deserialize<T>(reader);
        }
    }
}
