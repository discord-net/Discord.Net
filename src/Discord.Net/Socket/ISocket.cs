using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Socket
{
    public delegate ISocket SocketFactory(OnAbortionHandler abortionHandler, OnPacketHandler packetHandler);

    // A socket should only have one parent, so these do not need to be decoupled events.
    public delegate Task OnPacketHandler(object packet);
    public delegate void OnAbortionHandler(Exception error);

    public enum SocketState
    {
        Closed = default,
        AcquiringOpenLock,
        Opening,
        Open,
        AcquiringClosingLock,
        Closing,
        Aborted
    }

    public interface ISocket : IDisposable
    {
        SocketState State { get; }

        Task ConnectAsync(Uri uri, CancellationToken token);
        Task CloseAsync(int? code = null, string? reason = null);
        Task SendAsync(ReadOnlyMemory<byte> payload);

        OnAbortionHandler OnAbortion { get; }
        OnPacketHandler OnPacket { get; }
    }
}
