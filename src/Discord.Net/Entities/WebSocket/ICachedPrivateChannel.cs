using System.Collections.Generic;

namespace Discord
{
    internal interface ICachedPrivateChannel : ICachedChannel, IPrivateChannel
    {
        new IReadOnlyCollection<CachedDMUser> Recipients { get; }
    }
}
