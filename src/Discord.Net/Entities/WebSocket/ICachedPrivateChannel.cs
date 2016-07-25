using System.Collections.Generic;

namespace Discord
{
    internal interface ICachedPrivateChannel : ICachedChannel, IPrivateChannel
    {
        new IReadOnlyCollection<ICachedUser> Recipients { get; }
    }
}
