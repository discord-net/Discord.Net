using System;

namespace Discord
{
    public interface IGuildIntegration
    {
        /// <summary> Gets the integration ID. </summary>
        /// <returns> An <see cref="ulong"/> representing the unique identifier value of this integration. </returns>
        ulong Id { get; }
        /// <summary> Gets the integration name. </summary>
        /// <returns> A string containing the name of this integration. </returns>
        string Name { get; }
        /// <summary> Gets the integration type (twitch, youtube, etc). </summary>
        /// <returns> A string containing the name of the type of integration. </returns>
        string Type { get; }
        /// <summary> Gets if this integration is enabled or not. </summary>
        /// <summary> A value indicating if this integration is enabled. </returns>
        bool IsEnabled { get; }
        /// <summary> Gets if this integration is syncing or not. </summary>
        /// <returns> A value indicating if this integration is syncing. </returns>
        /// <remarks>
        ///     An integration with syncing enabled will update its "subscribers" on
        ///     an interval, while one with syncing disabled will not.
        ///     A user must manually choose when sync the integration
        ///     if syncing is disabled.
        /// </remarks>
        bool IsSyncing { get; }
        /// <summary> Gets the ID that this integration uses for "subscribers". </summary>
        ulong ExpireBehavior { get; }
        /// <summary> Gets the grace period before expiring "subscribers". </summary>
        ulong ExpireGracePeriod { get; }
        /// <summary> Gets when this integration was last synced. </summary>
        /// <returns> A <see cref="DateTimeOffset"/> containing a date and time of day when the integration was last synced. </returns>
        DateTimeOffset SyncedAt { get; }
        /// <summary>
        ///     Gets integration account information.
        /// </summary>
        IntegrationAccount Account { get; }

        IGuild Guild { get; }
        ulong GuildId { get; }
        ulong RoleId { get; }
        IUser User { get; }
    }
}
