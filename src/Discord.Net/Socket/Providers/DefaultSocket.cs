using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Socket.Providers
{
    public static class DefaultSocketFactory
    {
        public static ISocket Create(OnAbortionHandler onAbortion, OnPacketHandler onPacket)
        {
            return new DefaultSocket(onAbortion, onPacket);
        }
    }

    internal class DefaultSocket : ISocket
    {
        public SocketState State { get; private set; }
        public OnAbortionHandler OnAbortion { get; }
        public OnPacketHandler OnPacket { get; }

        private ClientWebSocket _socket;
        private Task? _receiveTask;

        private CancellationTokenSource _cancelTokenSource;
        private SemaphoreSlim _sendLock;
        private SemaphoreSlim _stateLock;

        public DefaultSocket(OnAbortionHandler onAbortion, OnPacketHandler onPacket)
        {
            _socket = new ClientWebSocket();

            _cancelTokenSource = new CancellationTokenSource();
            _sendLock = new SemaphoreSlim(1);
            _stateLock = new SemaphoreSlim(1);

            OnAbortion = onAbortion;
            OnPacket = onPacket;
        }

        public async Task ConnectAsync(Uri uri, CancellationToken connectCancelToken)
        {
            if (State == SocketState.Open
                || State == SocketState.Opening
                || State == SocketState.AcquiringOpenLock
                || State == SocketState.Aborted)
            {
                // todo: evaluate how to handle a (redundant?) state operation
                return;
            }

            CancellationTokenSource openLock; // create a linked token in case the caller wants to cancel an opening connection
            try
            {
                openLock = CancellationTokenSource.CreateLinkedTokenSource(_cancelTokenSource.Token, connectCancelToken);
            }
            catch (ObjectDisposedException e)
            {
                // Failed to link openLock, an expired cancellation token was passed
                State = SocketState.Aborted;
                OnAbortion(e);
                return;
            }

            State = SocketState.AcquiringOpenLock;
            try
            {
                await _stateLock.WaitAsync(openLock.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Failed to acquire openLock
                State = SocketState.Aborted;
                OnAbortion(e);
            }
            State = SocketState.Opening;

            try
            { 
                await _socket.ConnectAsync(uri, _cancelTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // Failed to open socket connection
                State = SocketState.Aborted;
                OnAbortion(e);
                return;
            }
            State = SocketState.Open;

            // TODO: this should not be expected to fail
            _stateLock.Release();
            openLock.Dispose();
        }
        public async Task CloseAsync(int? code, string? reason)
        {
            if (State == SocketState.Closed
                || State == SocketState.Closing
                || State == SocketState.AcquiringClosingLock
                || State == SocketState.Aborted)
            {
                // todo: evaluate how to handle a (redundant?) state operation; see OpenAsync
                return;
            }

            State = SocketState.AcquiringClosingLock;
            try
            {
                await _stateLock.WaitAsync();
            }
            catch (Exception e)
            {
                State = SocketState.Aborted;
                OnAbortion(e);
                return;
            }
            State = SocketState.Closing;
            
            // I think it is acceptable to use CancellationToken.None here, as no parallel operation should need to cancel the socket closure
            await _socket.CloseAsync((WebSocketCloseStatus)(code ?? 1005),
                reason ?? string.Empty,
                CancellationToken.None
            ).ConfigureAwait(false);

            // Wait until after .NET has been told to close the socket to cancel any pending sends/receives
            //
            // Presumably, sends/receives should have failed gracefully by this point, instead of aborting the underlying socket
            try
            {
                _cancelTokenSource.Cancel();
                await (_receiveTask ?? Task.CompletedTask);
            }
            catch
            {
                // just log for now
            }

            State = SocketState.Closed;
        }
        public async Task ReceiveAsync()
        {
            while (State == SocketState.Open && !_cancelTokenSource.IsCancellationRequested)
            {
                try
                {
                    Memory<byte> buffer = new Memory<byte>();
                    var res = await _socket.ReceiveAsync(buffer, _cancelTokenSource.Token).ConfigureAwait(false);
                    // todo: handle memory renting and ongoing messages
                    // todo: parse and OnPacket
                }
                catch (Exception err)
                {
                    // log error
                    if (_socket.State != WebSocketState.Open) // detrimental error
                    {
                        State = SocketState.Aborted;
                        OnAbortion(err);
                        return;
                    }
                }
            }
        }
        public async Task SendAsync(ReadOnlyMemory<byte> data)
        {
            if (State != SocketState.Open)
            {
                // raise error?
                return;
            }

            await _sendLock.WaitAsync().ConfigureAwait(false);
            try
            {
                // TODO: compression? who needs it
                await _socket.SendAsync(data, WebSocketMessageType.Text, true, _cancelTokenSource.Token).ConfigureAwait(false);
            }
            finally
            {
                _sendLock.Release();
            }
        }

        public void Dispose()
        {
            if (State != SocketState.Closed)
            {
                // log error? can this still proceed...
            }
            _socket.Dispose();
            _cancelTokenSource.Dispose();
            _stateLock.Dispose();
        }
    }
}
