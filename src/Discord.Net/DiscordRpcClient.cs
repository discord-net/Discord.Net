using Discord.API.Rpc;
using Discord.Logging;
using Discord.Net.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Discord
{
    public class DiscordRpcClient
    {
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        private readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        public event Func<Task> LoggedIn { add { _loggedInEvent.Add(value); } remove { _loggedInEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _loggedInEvent = new AsyncEvent<Func<Task>>();
        public event Func<Task> LoggedOut { add { _loggedOutEvent.Add(value); } remove { _loggedOutEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _loggedOutEvent = new AsyncEvent<Func<Task>>();

        public event Func<Task> Connected { add { _connectedEvent.Add(value); } remove { _connectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        public event Func<Exception, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();

        public event Func<Task> Ready { add { _readyEvent.Add(value); } remove { _readyEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _readyEvent = new AsyncEvent<Func<Task>>();
        
        private readonly ILogger _clientLogger, _rpcLogger;
        private readonly SemaphoreSlim _connectionLock;
        private readonly JsonSerializer _serializer;

        private TaskCompletionSource<bool> _connectTask;
        private CancellationTokenSource _cancelToken;
        internal SelfUser _currentUser;
        private Task _reconnectTask;
        private bool _isFirstLogSub;
        private bool _isReconnecting;
        private bool _isDisposed;

        public API.DiscordRpcApiClient ApiClient { get; }
        internal LogManager LogManager { get; }
        public LoginState LoginState { get; private set; }
        public ConnectionState ConnectionState { get; private set; }

        /// <summary> Creates a new RPC discord client. </summary>
        public DiscordRpcClient(string clientId, string origin) : this(new DiscordRpcConfig(clientId, origin)) { }
        /// <summary> Creates a new RPC discord client. </summary>
        public DiscordRpcClient(DiscordRpcConfig config)
        {
            LogManager = new LogManager(config.LogLevel);
            LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);
            _clientLogger = LogManager.CreateLogger("Client");
            _rpcLogger = LogManager.CreateLogger("RPC");
            _isFirstLogSub = true;

            _connectionLock = new SemaphoreSlim(1, 1);

            _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };
            _serializer.Error += (s, e) =>
            {
                _rpcLogger.WarningAsync(e.ErrorContext.Error).GetAwaiter().GetResult();
                e.ErrorContext.Handled = true;
            };

            ApiClient = new API.DiscordRpcApiClient(config.ClientId, config.Origin, config.WebSocketProvider);
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
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                ApiClient.Dispose();
                _isDisposed = true;
            }
        }
        public void Dispose() => Dispose(true);

        /// <inheritdoc />
        public async Task LoginAsync(TokenType tokenType, string token, bool validateToken = true)
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await LoginInternalAsync(tokenType, token, validateToken).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task LoginInternalAsync(TokenType tokenType, string token, bool validateToken)
        {
            if (_isFirstLogSub)
            {
                _isFirstLogSub = false;
                await WriteInitialLog().ConfigureAwait(false);
            }

            if (LoginState != LoginState.LoggedOut)
                await LogoutInternalAsync().ConfigureAwait(false);
            LoginState = LoginState.LoggingIn;

            try
            {
                await ApiClient.LoginAsync(tokenType, token).ConfigureAwait(false);

                LoginState = LoginState.LoggedIn;
            }
            catch (Exception)
            {
                await LogoutInternalAsync().ConfigureAwait(false);
                throw;
            }

            await _loggedInEvent.InvokeAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
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
            if (LoginState == LoginState.LoggedOut) return;
            LoginState = LoginState.LoggingOut;

            await ApiClient.LogoutAsync().ConfigureAwait(false);

            _currentUser = null;

            LoginState = LoginState.LoggedOut;

            await _loggedOutEvent.InvokeAsync().ConfigureAwait(false);
        }
        
        public async Task ConnectAsync()
        {
            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                _isReconnecting = false;
                await ConnectInternalAsync().ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ConnectInternalAsync(bool ignoreLoginCheck = false)
        {
            if (LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("You must log in before connecting or call ConnectAndAuthorizeAsync.");

            if (_isFirstLogSub)
            {
                _isFirstLogSub = false;
                await WriteInitialLog().ConfigureAwait(false);
            }

            var state = ConnectionState;
            if (state == ConnectionState.Connecting || state == ConnectionState.Connected)
                await DisconnectInternalAsync(null).ConfigureAwait(false);

            ConnectionState = ConnectionState.Connecting;
            await _rpcLogger.InfoAsync("Connecting").ConfigureAwait(false);
            try
            {
                _connectTask = new TaskCompletionSource<bool>();
                _cancelToken = new CancellationTokenSource();
                await ApiClient.ConnectAsync().ConfigureAwait(false);
                await _connectedEvent.InvokeAsync().ConfigureAwait(false);

                await _connectTask.Task.ConfigureAwait(false);

                ConnectionState = ConnectionState.Connected;
                await _rpcLogger.InfoAsync("Connected").ConfigureAwait(false);
            }
            catch (Exception)
            {
                await DisconnectInternalAsync(null).ConfigureAwait(false);
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
                await DisconnectInternalAsync(null).ConfigureAwait(false);
            }
            finally { _connectionLock.Release(); }
        }
        private async Task DisconnectInternalAsync(Exception ex)
        {
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
            //TODO: Is this thread-safe?
            if (_reconnectTask != null) return;

            await _connectionLock.WaitAsync().ConfigureAwait(false);
            try
            {
                await DisconnectInternalAsync(ex).ConfigureAwait(false);
                if (_reconnectTask != null) return;
                _isReconnecting = true;
                _reconnectTask = ReconnectInternalAsync();
            }
            finally { _connectionLock.Release(); }
        }
        private async Task ReconnectInternalAsync()
        {
            try
            {
                int nextReconnectDelay = 1000;
                while (_isReconnecting)
                {
                    try
                    {
                        await Task.Delay(nextReconnectDelay).ConfigureAwait(false);
                        nextReconnectDelay *= 2;
                        if (nextReconnectDelay > 30000)
                            nextReconnectDelay = 30000;

                        await _connectionLock.WaitAsync().ConfigureAwait(false);
                        try
                        {
                            await ConnectInternalAsync().ConfigureAwait(false);
                        }
                        finally { _connectionLock.Release(); }
                        return;
                    }
                    catch (Exception ex)
                    {
                        await _rpcLogger.WarningAsync("Reconnect failed", ex).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                await _connectionLock.WaitAsync().ConfigureAwait(false);
                try
                {
                    _isReconnecting = false;
                    _reconnectTask = null;
                }
                finally { _connectionLock.Release(); }
            }
        }

        public async Task<string> AuthorizeAsync(string[] scopes)
        {
            await ConnectAsync().ConfigureAwait(false);
            var result = await ApiClient.SendAuthorizeAsync(scopes).ConfigureAwait(false);
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
