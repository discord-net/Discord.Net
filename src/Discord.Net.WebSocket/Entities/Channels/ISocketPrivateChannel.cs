using System.Collections.Generic;

namespace Discord.WebSocket
{
    public interface ISocketPrivateChannel : IPrivateChannel
    {
        new IReadOnlyCollection<SocketUser> Recipients { get; }
    }
}
