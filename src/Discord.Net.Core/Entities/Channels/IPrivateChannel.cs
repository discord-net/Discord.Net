using System.Collections.Generic;

namespace Discord
{
    public interface IPrivateChannel : IChannel
    {
        IReadOnlyCollection<IUser> Recipients { get; }
    }
}
