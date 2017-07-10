using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Udp
{
    public interface IUdpSocket
    {
        event Func<byte[], int, int, Task> ReceivedDatagram;

        ushort Port { get; }

        void SetCancelToken(CancellationToken cancelToken);
        void SetDestination(string ip, int port);

        Task StartAsync();
        Task StopAsync();

        Task SendAsync(byte[] data, int index, int count);
    }
}
