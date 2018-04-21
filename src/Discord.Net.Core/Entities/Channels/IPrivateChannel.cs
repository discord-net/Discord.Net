using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a generic channel that is private to select recipients.
    /// </summary>
    public interface IPrivateChannel : IChannel
    {
        /// <summary>
        ///     Users that can access this channel.
        /// </summary>
        IReadOnlyCollection<IUser> Recipients { get; }
    }
}
