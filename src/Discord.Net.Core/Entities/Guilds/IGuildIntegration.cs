using System;

namespace Discord
{
    public interface IGuildIntegration
    {
        /// <summary>
        ///     The integration ID.
        /// </summary>
        ulong Id { get; }
        /// <summary>
        ///     The integration name.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     The integration type (twich, youtube, etc).
        /// </summary>
        string Type { get; }
        /// <summary>
        ///     Is this integration enabled?
        /// </summary>
        bool IsEnabled { get; }
        /// <summary>
        ///     Is this integration syncing?
        /// </summary>
        bool IsSyncing { get; }
        /// <summary>
        ///     ID that this integration uses for "subscribers".
        /// </summary>
        ulong ExpireBehavior { get; }
        /// <summary>
        ///     The grace period before expiring subscribers.
        /// </summary>
        ulong ExpireGracePeriod { get; }
        /// <summary>
        ///     When this integration was last synced.
        /// </summary>
        DateTimeOffset SyncedAt { get; }
        /// <summary>
        ///     Integration account information. See <see cref="IntegrationAccount"/>.
        /// </summary>
        IntegrationAccount Account { get; }

        IGuild Guild { get; }
        ulong GuildId { get; }
        ulong RoleId { get; }
        IUser User { get; }
    }
}
