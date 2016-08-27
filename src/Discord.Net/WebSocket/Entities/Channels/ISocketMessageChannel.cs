using System.Collections.Generic;
using MessageModel = Discord.API.Message;

namespace Discord.WebSocket
{
    internal interface ISocketMessageChannel : ISocketChannel, IMessageChannel
    {
        IReadOnlyCollection<ISocketUser> Users { get; }

        ISocketMessage CreateMessage(ISocketUser author, MessageModel model);
        ISocketMessage AddMessage(ISocketUser author, MessageModel model);
        ISocketMessage GetMessage(ulong id);
        ISocketMessage RemoveMessage(ulong id);

        ISocketUser GetUser(ulong id, bool skipCheck = false);
    }
}
