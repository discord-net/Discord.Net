using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents the user's presence status. This may include their online status and their activity.
    /// </summary>
    public interface IPresence
    {
        /// <summary>
        ///     Gets the current status of this user.
        /// </summary>
        UserStatus Status { get; }
        /// <summary>
        ///     Gets the set of clients where this user is currently active.
        /// </summary>
        IReadOnlyCollection<ClientType> ActiveClients { get; }
        /// <summary>
        ///     Gets the list of activities that this user currently has available.
        /// </summary>
        IReadOnlyCollection<IActivity> Activities { get; }
    }
}
