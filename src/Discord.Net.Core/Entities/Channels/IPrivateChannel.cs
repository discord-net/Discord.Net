using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents a generic channel that is private to select recipients.
    /// </summary>
    public interface IPrivateChannel : IChannel
    {
        /// <summary>
        ///     Gets the users that can access this channel.
        /// </summary>
        /// <returns>
        ///     A read-only collection of users that can access this channel.
        /// </returns>
        IReadOnlyCollection<IUser> Recipients { get; }
    }
}
