using System.Collections.Generic;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a generic WebSocket-based channel that is private to select recipients.
    /// </summary>
    public interface ISocketPrivateChannel : IPrivateChannel
    {
        new IReadOnlyCollection<SocketUser> Recipients { get; }
    }
}
