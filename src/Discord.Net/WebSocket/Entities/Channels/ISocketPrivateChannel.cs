using System.Collections.Generic;

namespace Discord.WebSocket
{
    internal interface ISocketPrivateChannel : ISocketChannel, IPrivateChannel
    {
        new IReadOnlyCollection<ISocketUser> Recipients { get; }
    }
}
