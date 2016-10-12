using System.Collections.Generic;

namespace Discord.Rest
{
    public interface IRestPrivateChannel : IPrivateChannel
    {
        new IReadOnlyCollection<RestUser> Recipients { get; }
    }
}
