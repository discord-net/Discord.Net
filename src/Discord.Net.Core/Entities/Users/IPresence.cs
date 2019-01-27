using System.Collections.Immutable;

namespace Discord
{
    /// <summary>
    ///     Represents the user's presence status. This may include their online status and their activity.
    /// </summary>
    public interface IPresence
    {
        /// <summary>
        ///     Gets the activity this user is currently doing.
        /// </summary>
        IActivity Activity { get; }
        /// <summary>
        ///     Gets the current status of this user.
        /// </summary>
        UserStatus Status { get; }
        /// <summary>
        ///     Gets the set of clients where this user is currently active.
        /// </summary>
        // TODO: Un-comment ActiveClients definition in IPresence for 2.1.x breaking update
        // IImmutableSet<ClientType> ActiveClients { get; }
    }
}
