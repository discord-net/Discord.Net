using System.Collections.Generic;
using MessageModel = Discord.API.Message;

namespace Discord
{
    internal interface ISocketMessageChannel : ISocketChannel, IMessageChannel
    {
        IReadOnlyCollection<ISocketUser> Users { get; }

        SocketMessage AddMessage(ISocketUser author, MessageModel model);
        SocketMessage GetMessage(ulong id);
        SocketMessage RemoveMessage(ulong id);

        ISocketUser GetUser(ulong id, bool skipCheck = false);
    }
}
