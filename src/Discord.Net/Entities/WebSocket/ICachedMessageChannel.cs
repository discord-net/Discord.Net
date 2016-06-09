using System.Collections.Generic;
using MessageModel = Discord.API.Message;

namespace Discord
{
    internal interface ICachedMessageChannel : ICachedChannel, IMessageChannel
    {
        IReadOnlyCollection<ICachedUser> Members { get; }

        CachedMessage AddCachedMessage(ICachedUser author, MessageModel model);
        new CachedMessage GetCachedMessage(ulong id);
        CachedMessage RemoveCachedMessage(ulong id);

        ICachedUser GetCachedUser(ulong id);
    }
}
