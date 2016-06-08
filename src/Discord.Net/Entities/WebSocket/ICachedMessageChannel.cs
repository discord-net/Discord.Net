using System.Collections.Generic;
using MessageModel = Discord.API.Message;

namespace Discord
{
    internal interface ICachedMessageChannel : ICachedChannel, IMessageChannel
    {
        IReadOnlyCollection<IUser> Members { get; }

        CachedMessage AddCachedMessage(IUser author, MessageModel model);
        new CachedMessage GetCachedMessage(ulong id);
        CachedMessage RemoveCachedMessage(ulong id);

        IUser GetCachedUser(ulong id);
    }
}
