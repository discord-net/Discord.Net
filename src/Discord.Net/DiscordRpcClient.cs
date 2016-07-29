using Discord.API.Rpc;
using Discord.Logging;
using Discord.Net.Converters;
using Discord.Net.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    public class DiscordRpcClient : DiscordRestClient
    {
        public event Func<Task> Connected { add { _connectedEvent.Add(value); } remove { _connectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();

        public event Func<Task> Ready { add { _readyEvent.Add(value); } remove { _readyEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _readyEvent = new AsyncEvent<Func<Task>>();
        
        private readonly ILogger _rpcLogger;
        private readonly JsonSerializer _serializer;

        private TaskCompletionSource<bool> _connectTask;
        private CancellationTokenSource _cancelToken, _reconnectCancelToken;
        private Task _reconnectTask;
        private bool _isFirstLogSub;
        private bool _isReconnecting;
        private bool _canReconnect;

        public ConnectionState ConnectionState { get; private set; }

        public new API.DiscordRpcApiClient ApiClient => base.ApiClient as API.DiscordRpcApiClient;

        /// <summary> Creates a new RPC discord client. </summary>
        public DiscordRpcClient(string clientId, string origin) : this(new DiscordRpcConfig(clientId, origin)) { }
        /// <summary> Creates a new RPC discord client. </summary>
        public DiscordRpcClient(DiscordRpcConfig config)
            : base(config, CreateApiClient(config))
        {
            _rpcLogger = LogManager.CreateLogger("RPC");
            _isFirstLogSub = true;

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _rpcLogger.WarningAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };
            
            ApiClient.SentRpcMessage += async opCode => await _rpcLogger.DebugAsync($"Sent {opCode}").ConfigureAwait(false);
            ApiClient.ReceivedRpcEvent += ProcessMessageAsync;
            ApiClient.Disconnected += async ex =>
            {
                if (ex != null)
                {
                    await _rpcLogger.WarningAsync($"Connection Closed: {ex.Message}").ConfigureAwait(false);
                    await StartReconnectAsync(ex).ConfigureAwait(false);
                }
                else
                    await _rpcLogger.WarningAsync($"Connection Closed").ConfigureAwait(false);
            };
        }
        private static API.DiscordRpcApiClient CreateApiClient(DiscordRpcConfig config)
            => new API.DiscordRpcApiClient(config.ClientId, config.Origin, config.RestClientProvider, config.WebSocketProvider, requestQueue: new RequestQueue());

        internal override void Dispose(bool disposing)
        {
            if (!_isDisposed)
                ApiClient.Dispose();
        }

        protected override Task ValidateTokenAsync(TokenType tokenType, string token)
        {
            return Task.CompletedTask; //Validation is done in DiscordRpcAPIClient
        }

        /// <inheritdoc />
        public Task ConnectAsync() => ConnectAsync(false);
        internal async Task ConnectAsync(bool ignoreLoginCheck)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _isReconnecting = false;
                await ConnectInternalAsync(ignoreLoginCheck, false).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync(bool ignoreLoginCheck, bool isReconnecting)
        {
            if (!ignoreLoginCheck && LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("You must log in before connecting.");
            
            if (!isReconnecting && _reconnectCancelToken != null && !_reconnectCancelToken.IsCancellationRequested)
                _reconnectCancelToken.Cancel();

            if (_isFirstLogSub)
            {
                _isFirstLogSub = false;
                await WriteInitialLog().ConfigureAwait(false);
            }

            var state = ConnectionState;
            if (state == ConnectionState.Connecting || state == ConnectionState.Connected)
                await DisconnectInternalAsync(null, isReconnecting).ConfigureAwait(false);

            ConnectionState = ConnectionState.Connecting;
            await _rpcLogger.InfoAsync("Connecting").ConfigureAwait(false);
            try
            {
                _connectTask = new TaskCompletionSource<bool>();
                _cancelToken = new CancellationTokenSource();
                await ApiClient.ConnectAsync().ConfigureAwait(false);
                await _connectedEvent.InvokeAsync().ConfigureAwait(false);

                await _connectTask.Task.ConfigureAwait(false);
                _canReconnect = true;
                ConnectionState = ConnectionState.Connected;
                await _rpcLogger.InfoAsync("Connected").ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DisconnectInternalAsync(null, isReconnecting).ConfigureAwait(false);
                throw;
            }
        }
        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _isReconnecting = false;
                await DisconnectInternalAsync(null, false).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync(Exception ex, bool isReconnecting)
        {
            if (!isReconnecting)
            {
                _canReconnect = false;

                if (_reconnectCancelToken != null && !_reconnectCancelToken.IsCancellationRequested)
                    _reconnectCancelToken.Cancel();
            }

            if (ConnectionState == ConnectionState.Disconnected) return;
            ConnectionState = ConnectionState.Disconnecting;
            await _rpcLogger.InfoAsync("Disconnecting").ConfigureAwait(false);

            await _rpcLogger.DebugAsync("Disconnecting - CancelToken").ConfigureAwait(false);
            //Signal tasks to complete
            try { _cancelToken.Cancel(); } catch { }

            await _rpcLogger.DebugAsync("Disconnecting - ApiClient").ConfigureAwait(false);
            //Disconnect from server
            await ApiClient.DisconnectAsync().ConfigureAwait(false);
            
            ConnectionState = ConnectionState.Disconnected;
            await _rpcLogger.InfoAsync("Disconnected").ConfigureAwait(false);

            await _disconnectedEvent.InvokeAsync(ex).ConfigureAwait(false);
        }

        private async Task StartReconnectAsync(Exception ex)
        {
            _connectTask?.TrySetException(ex);
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!_canReconnect || _reconnectTask != null) return;
                await DisconnectInternalAsync(null, true).ConfigureAwait(false);
                _reconnectCancelToken = new CancellationTokenSource();
                _reconnectTask = ReconnectInternalAsync(_reconnectCancelToken.Token);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ReconnectInternalAsync(CancellationToken cancelToken)
        {
            try
            {
                Random jitter = new Random();
                int nextReconnectDelay = 1000;
                while (true)
                {
                    await Task.Delay(nextReconnectDelay, cancelToken).ConfigureAwait(false);
                    nextReconnectDelay = nextReconnectDelay * 2 + jitter.Next(-250, 250);
                    if (nextReconnectDelay > 60000)
                        nextReconnectDelay = 60000;

                    await _connectionLock.WaitAsync().ConfigureAwait(false);
                    try
                    {
                        if (cancelToken.IsCancellationRequested) return;
                        await ConnectInternalAsync(false, true).ConfigureAwait(false);
                        _reconnectTask = null;
                        return;
                    }
                    catch (Exception ex)
                    {
                        await _rpcLogger.WarningAsync("Reconnect failed", ex).ConfigureAwait(false);
                    }
                    finally { _connectionLock.Release(); }
                }
            }
            catch (OperationCanceledException)
            {
                await _connectionLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    await _rpcLogger.DebugAsync("Reconnect cancelled").ConfigureAwait(false);
                    _reconnectTask = null;
                }
                finally { _connectionLock.Release(); }
            }
        }

        public async Task<string> AuthorizeAsync(string[] scopes, string rpcToken = null)
        {
            await ConnectAsync(true).ConfigureAwait(false);
            var result = await ApiClient.SendAuthorizeAsync(scopes, rpcToken).ConfigureAwait(false);
            await DisconnectAsync().ConfigureAwait(false);
            return result.Code;
        }

        private async Task ProcessMessageAsync(string cmd, Optional<string> evnt, Optional<object> payload)
        {
            try
            {
                switch (cmd)
                {
                    case "DISPATCH":
                        switch (evnt.Value)
                        {
                            //Connection
                            case "READY":
                                {
                                    await _rpcLogger.DebugAsync("Received Dispatch (READY)").ConfigureAwait(false);
                                    var data = (payload.Value as JToken).ToObject<ReadyEvent>(_serializer);
                                    var cancelToken = _cancelToken;

                                    var _ = Task.Run(async () =>
                                    {
                                        try
                                        {
                                            RequestOptions options = new RequestOptions
                                            {
                                                //CancellationToken = cancelToken //TODO: Implement
                                            };
                                            
                                            if (LoginState != LoginState.LoggedOut)
                                                await ApiClient.SendAuthenticateAsync(options).ConfigureAwait(false); //Has bearer

                                            var __ = _connectTask.TrySetResultAsync(true); //Signal the .Connect() call to complete
                                            await _rpcLogger.InfoAsync("Ready").ConfigureAwait(false);
                                        }
                                        catch (Exception ex)
                                        {
                                            await _rpcLogger.ErrorAsync($"Error handling {cmd}{(evnt.IsSpecified ? $" ({evnt})" : "")}", ex).ConfigureAwait(false);
                                            return;
                                        }
                                    });
                                }
                                break;

                            //Others
                            default:
                                await _rpcLogger.WarningAsync($"Unknown Dispatch ({evnt})").ConfigureAwait(false);
                                return;
                        }
                        break;

                    /*default:
                        await _rpcLogger.WarningAsync($"Unknown OpCode ({cmd})").ConfigureAwait(false);
                        return;*/
                }
            }
            catch (Exception ex)
            {
                await _rpcLogger.ErrorAsync($"Error handling {cmd}{(evnt.IsSpecified ? $" ({evnt})" : "")}", ex).ConfigureAwait(false);
                return;
            }
        }

        private async Task WriteInitialLog()
        {
            await _clientLogger.InfoAsync($"DiscordRpcClient v{DiscordRestConfig.Version} (RPC v{DiscordRpcConfig.RpcAPIVersion})").ConfigureAwait(false);
            await _clientLogger.VerboseAsync($"Runtime: {RuntimeInformation.FrameworkDescription.Trim()} ({ToArchString(RuntimeInformation.ProcessArchitecture)})").ConfigureAwait(false);
            await _clientLogger.VerboseAsync($"OS: {RuntimeInformation.OSDescription.Trim()} ({ToArchString(RuntimeInformation.OSArchitecture)})").ConfigureAwait(false);
            await _clientLogger.VerboseAsync($"Processors: {Environment.ProcessorCount}").ConfigureAwait(false);
        }
        private static string ToArchString(Architecture arch)
        {
            switch (arch)
            {
                case Architecture.X64: return "x64";
                case Architecture.X86: return "x86";
                default: return arch.ToString();
            }
        }
    }
}
