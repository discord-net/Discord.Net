using System.Collections.Generic;
using MessageModel = Discord.API.Message;

namespace Discord
{
    internal interface ICachedMessageChannel : ICachedChannel, IMessageChannel
    {
        IReadOnlyCollection<ICachedUser> Members { get; }

        CachedMessage AddMessage(ICachedUser author, MessageModel model);
        CachedMessage GetMessage(ulong id);
        CachedMessage RemoveMessage(ulong id);

        ICachedUser GetUser(ulong id, bool skipCheck = false);
    }
}
