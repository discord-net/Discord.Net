#if NETSTANDARD1_3
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Udp
{
    internal class DefaultUdpSocket : IUdpSocket, IDisposable
    {
        public event Func<byte[], int, int, Task> ReceivedDatagram;

        private readonly SemaphoreSlim _lock;
        private UdpClient _udp;
        private IPEndPoint _destination;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken, _parentToken;
        private Task _task;
        private bool _isDisposed;

        public DefaultUdpSocket()
        {
            _lock = new SemaphoreSlim(1, 1);
            _cancelTokenSource = new CancellationTokenSource();
        }
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                    StopInternalAsync(true).GetAwaiter().GetResult();
                _isDisposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }


        public async Task StartAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await StartInternalAsync(_cancelToken).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }
        public async Task StartInternalAsync(CancellationToken cancelToken)
        {
            await StopInternalAsync().ConfigureAwait(false);

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token).Token;

            _udp = new UdpClient(0);

            _task = RunAsync(_cancelToken);
        }
        public async Task StopAsync()
        {
            await _lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await StopInternalAsync().ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }
        public async Task StopInternalAsync(bool isDisposing = false)
        {
            try { _cancelTokenSource.Cancel(false); } catch { }

            if (!isDisposing)
                await (_task ?? Task.Delay(0)).ConfigureAwait(false);

            if (_udp != null)
            {
                try { _udp.Dispose(); }
                catch { }
                _udp = null;
            }
        }

        public void SetDestination(string ip, int port)
        {
            _destination = new IPEndPoint(IPAddress.Parse(ip), port);
        }
        public void SetCancelToken(CancellationToken cancelToken)
        {
            _parentToken = cancelToken;
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token).Token;
        }

        public async Task SendAsync(byte[] data, int index, int count)
        {
            if (index != 0) //Should never happen?
            {
                var newData = new byte[count];
                Buffer.BlockCopy(data, index, newData, 0, count);
                data = newData;
            }
            await _udp.SendAsync(data, count, _destination).ConfigureAwait(false);
        }

        private async Task RunAsync(CancellationToken cancelToken)
        {
            var closeTask = Task.Delay(-1, cancelToken);
            while (!cancelToken.IsCancellationRequested)
            {
                var receiveTask = _udp.ReceiveAsync();
                var task = await Task.WhenAny(closeTask, receiveTask).ConfigureAwait(false);
                if (task == closeTask)
                    break;

                var result = receiveTask.Result;
                await ReceivedDatagram(result.Buffer, 0, result.Buffer.Length).ConfigureAwait(false);
            }
        }
    }
}
#endif