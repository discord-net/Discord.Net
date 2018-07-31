using System;

namespace Discord
{
    public interface IGuildIntegration
    {
        /// <summary> Gets the integration ID. </summary>
        /// <returns> Gets the integration ID. </returns>
        ulong Id { get; }
        /// <summary> Gets the integration name. </summary>
        /// <returns> Gets the integration name. </returns>
        string Name { get; }
        /// <summary> Gets the integration type (twitch, youtube, etc). </summary>
        /// <returns> Gets the integration type (twitch, youtube, etc). </returns>
        string Type { get; }
        /// <summary> Gets if this integration is enabled or not. </summary>
        /// <summary> Gets if this integration is enabled or not. </returns>
        bool IsEnabled { get; }
        /// <summary> Gets if this integration is syncing or not. </summary>
        /// <returns> Gets if this integration is syncing or not. </returns>
        bool IsSyncing { get; }
        /// <summary> Gets the ID that this integration uses for "subscribers". </summary>
        /// <returns> Gets the ID that this integration uses for "subscribers". </returns>
        ulong ExpireBehavior { get; }
        /// <summary> Gets the grace period before expiring subscribers. </summary>
        /// <returns> Gets the grace period before expiring subscribers. </returns>
        ulong ExpireGracePeriod { get; }
        /// <summary> Gets when this integration was last synced. </summary>
        /// <returns> Gets when this integration was last synced. </returns>
        DateTimeOffset SyncedAt { get; }
        /// <summary>
        ///     Gets integration account information. See <see cref="IntegrationAccount"/>.
        /// </summary>
        /// <returns>
        ///     Gets integration account information. See <see cref="IntegrationAccount"/>.
        /// </returns>
        IntegrationAccount Account { get; }

        IGuild Guild { get; }
        ulong GuildId { get; }
        ulong RoleId { get; }
        IUser User { get; }
    }
}
