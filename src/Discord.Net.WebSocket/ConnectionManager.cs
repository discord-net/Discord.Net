using Discord.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Net;

namespace Discord
{
    internal class ConnectionManager
    {
        public event Func<Task> Connected { add { _connectedEvent.Add(value); } remove { _connectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        public event Func<Exception, bool, Task> Disconnected { add { _disconnectedEvent.Add(value); } remove { _disconnectedEvent.Remove(value); } }
        private readonly AsyncEvent<Func<Exception, bool, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, bool, Task>>();

        private readonly SemaphoreSlim _stateLock;
        private readonly Logger _logger;
        private readonly int _connectionTimeout;
        private readonly Func<Task> _onConnecting;
        private readonly Func<Exception, Task> _onDisconnecting;

        private TaskCompletionSource<bool> _connectionPromise, _readyPromise;
        private CancellationTokenSource _combinedCancelToken, _reconnectCancelToken, _connectionCancelToken;
        private Task _task;

        public ConnectionState State { get; private set; }
        public CancellationToken CancelToken { get; private set; }

        internal ConnectionManager(SemaphoreSlim stateLock, Logger logger, int connectionTimeout, 
            Func<Task> onConnecting, Func<Exception, Task> onDisconnecting, Action<Func<Exception, Task>> clientDisconnectHandler)
        {
            _stateLock = stateLock;
            _logger = logger;
            _connectionTimeout = connectionTimeout;
            _onConnecting = onConnecting;
            _onDisconnecting = onDisconnecting;

            clientDisconnectHandler(ex =>
            {
                if (ex != null)
                {
                    var ex2 = ex as WebSocketClosedException;
                    if (ex2?.CloseCode == 4006)
                        CriticalError(new Exception("WebSocket session expired", ex));
                    else
                        Error(new Exception("WebSocket connection was closed", ex));
                }
                else
                    Error(new Exception("WebSocket connection was closed"));
                return Task.Delay(0);
            });
        }

        public virtual async Task StartAsync()
        {
            await AcquireConnectionLock().ConfigureAwait(false);
            var reconnectCancelToken = new CancellationTokenSource();
            _reconnectCancelToken = reconnectCancelToken;
            _task = Task.Run(async () =>
            {
                try
                {
                    Random jitter = new Random();
                    int nextReconnectDelay = 1000;
                    while (!reconnectCancelToken.IsCancellationRequested)
                    {
                        try
                        {
                            await ConnectAsync(reconnectCancelToken).ConfigureAwait(false);
                            nextReconnectDelay = 1000; //Reset delay                          
                            await _connectionPromise.Task.ConfigureAwait(false);
                        }
                        catch (OperationCanceledException ex) 
                        { 
                            Cancel(); //In case this exception didn't come from another Error call
                            await DisconnectAsync(ex, !reconnectCancelToken.IsCancellationRequested).ConfigureAwait(false);
                        }
                        catch (Exception ex) 
                        { 
                            Error(ex); //In case this exception didn't come from another Error call
                            if (!reconnectCancelToken.IsCancellationRequested)
                            {
                                await _logger.WarningAsync(ex).ConfigureAwait(false);
                                await DisconnectAsync(ex, true).ConfigureAwait(false);
                            }
                            else
                            {
                                await _logger.ErrorAsync(ex).ConfigureAwait(false);
                                await DisconnectAsync(ex, false).ConfigureAwait(false);
                            }
                        }

                        if (!reconnectCancelToken.IsCancellationRequested)
                        {
                            //Wait before reconnecting
                            await Task.Delay(nextReconnectDelay, reconnectCancelToken.Token).ConfigureAwait(false);
                            nextReconnectDelay = (nextReconnectDelay * 2) + jitter.Next(-250, 250);
                            if (nextReconnectDelay > 60000)
                                nextReconnectDelay = 60000;
                        }
                    }
                }
                finally { _stateLock.Release(); }
            });
        }
        public virtual async Task StopAsync()
        {
            Cancel();
            var task = _task;
            if (task != null)
                await task.ConfigureAwait(false);
        }

        private async Task ConnectAsync(CancellationTokenSource reconnectCancelToken)
        {
            _connectionCancelToken = new CancellationTokenSource();
            _combinedCancelToken = CancellationTokenSource.CreateLinkedTokenSource(_connectionCancelToken.Token, reconnectCancelToken.Token);
            CancelToken = _combinedCancelToken.Token;

            _connectionPromise = new TaskCompletionSource<bool>();
            State = ConnectionState.Connecting;
            await _logger.InfoAsync("Connecting").ConfigureAwait(false);
            
            try
            {
                var readyPromise = new TaskCompletionSource<bool>();
                _readyPromise = readyPromise;

                //Abort connection on timeout
                var cancelToken = CancelToken;
                var _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(_connectionTimeout, cancelToken).ConfigureAwait(false);
                        readyPromise.TrySetException(new TimeoutException());
                    }
                    catch (OperationCanceledException) { }
                });

                await _onConnecting().ConfigureAwait(false);

                await _logger.InfoAsync("Connected").ConfigureAwait(false);
                State = ConnectionState.Connected;
                await _logger.DebugAsync("Raising Event").ConfigureAwait(false);
                await _connectedEvent.InvokeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Error(ex);
                throw;
            }
        }
        private async Task DisconnectAsync(Exception ex, bool isReconnecting)
        {
            if (State == ConnectionState.Disconnected) return;
            State = ConnectionState.Disconnecting;
            await _logger.InfoAsync("Disconnecting").ConfigureAwait(false);

            await _onDisconnecting(ex).ConfigureAwait(false);

            await _logger.InfoAsync("Disconnected").ConfigureAwait(false);
            State = ConnectionState.Disconnected;
            await _disconnectedEvent.InvokeAsync(ex, isReconnecting).ConfigureAwait(false);
        }

        public async Task CompleteAsync()
        {
            await _readyPromise.TrySetResultAsync(true).ConfigureAwait(false);
        }
        public async Task WaitAsync()
        {
            await _readyPromise.Task.ConfigureAwait(false);
        }

        public void Cancel()
        {
            _readyPromise?.TrySetCanceled();
            _connectionPromise?.TrySetCanceled();
            _reconnectCancelToken?.Cancel();
            _connectionCancelToken?.Cancel();
        }
        public void Error(Exception ex)
        {
            _readyPromise.TrySetException(ex);
            _connectionPromise.TrySetException(ex);
            _connectionCancelToken?.Cancel();
        }
        public void CriticalError(Exception ex)
        {
            _reconnectCancelToken?.Cancel();
            Error(ex);
        }
        public void Reconnect()
        {
            _readyPromise.TrySetCanceled();
            _connectionPromise.TrySetCanceled();
            _connectionCancelToken?.Cancel();
        }
        private async Task AcquireConnectionLock()
        {
            while (true)
            {
                await StopAsync().ConfigureAwait(false);
                if (await _stateLock.WaitAsync(0).ConfigureAwait(false))
                    break;
            }
        }
    }
}