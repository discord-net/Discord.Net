using System;

namespace Discord
{
    /// <summary>
    ///     Holds information for an integration feature.
    ///     Nullable fields not provided for Discord bot integrations, but are for Twitch etc.
    /// </summary>
    public interface IIntegration
    {
        /// <summary>
        ///     Gets the integration ID.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the unique identifier value of this integration.
        /// </returns>
        ulong Id { get; }
        /// <summary>
        ///     Gets the integration name.
        /// </summary>
        /// <returns>
        ///     A string containing the name of this integration.
        /// </returns>
        string Name { get; }
        /// <summary>
        ///     Gets the integration type (Twitch, YouTube, etc).
        /// </summary>
        /// <returns>
        ///     A string containing the name of the type of integration.
        /// </returns>
        string Type { get; }
        /// <summary>
        ///     Gets a value that indicates whether this integration is enabled or not.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this integration is enabled; otherwise <c>false</c>.
        /// </returns>
        bool IsEnabled { get; }
        /// <summary>
        ///     Gets a value that indicates whether this integration is syncing or not.
        /// </summary>
        /// <remarks>
        ///     An integration with syncing enabled will update its "subscribers" on an interval, while one with syncing
        ///     disabled will not. A user must manually choose when sync the integration if syncing is disabled.
        /// </remarks>
        /// <returns>
        ///      <c>true</c> if this integration is syncing; otherwise <c>false</c>.
        /// </returns>
        bool? IsSyncing { get; }
        /// <summary>
        ///     Gets the ID that this integration uses for "subscribers".
        /// </summary>
        ulong? RoleId { get; }
        /// <summary>
        ///     Gets whether emoticons should be synced for this integration (twitch only currently).
        /// </summary>
        bool? HasEnabledEmoticons { get; }
        /// <summary>
        ///     Gets the behavior of expiring subscribers.
        /// </summary>
        IntegrationExpireBehavior? ExpireBehavior { get; }
        /// <summary>
        ///     Gets the grace period before expiring "subscribers".
        /// </summary>
        int? ExpireGracePeriod { get; }
        /// <summary>
        ///     Gets the user for this integration.
        /// </summary>
        IUser User { get; }
        /// <summary>
        ///     Gets integration account information.
        /// </summary>
        IIntegrationAccount Account { get; }
        /// <summary>
        ///     Gets when this integration was last synced.
        /// </summary>
        /// <returns>
        ///     A <see cref="DateTimeOffset"/> containing a date and time of day when the integration was last synced.
        /// </returns>
        DateTimeOffset? SyncedAt { get; }
        /// <summary>
        ///     Gets how many subscribers this integration has.
        /// </summary>
        int? SubscriberCount { get; }
        /// <summary>
        ///     Gets whether this integration been revoked.
        /// </summary>
        bool? IsRevoked { get; }
        /// <summary>
        ///     Gets the bot/OAuth2 application for a discord integration.
        /// </summary>
        IIntegrationApplication Application { get; }

        IGuild Guild { get; }
        ulong GuildId { get; }
    }
}
