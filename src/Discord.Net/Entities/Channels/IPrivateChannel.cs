using System.Collections.Generic;

namespace Discord
{
    public interface IPrivateChannel
    {
        IReadOnlyCollection<IUser> Recipients { get; }
    }
}
