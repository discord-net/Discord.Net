using System.Collections.Generic;

namespace Discord
{
    internal interface ISocketPrivateChannel : ISocketChannel, IPrivateChannel
    {
        new IReadOnlyCollection<ISocketUser> Recipients { get; }
    }
}
