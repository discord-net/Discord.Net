using System;

namespace Discord.Net.Models
{
    /// <summary>
    /// Declares a flag enum which represents the system channel flags for a <see cref="Guild"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-object-system-channel-flags"/>
    /// </remarks>
    [Flags]
    public enum SystemChannelFlags
    {
        /// <summary>
        /// Suppress member join notifications.
        /// </summary>
        SuppressJoinNotifications = 1 << 0,

        /// <summary>
        /// Suppress server boost notifications.
        /// </summary>
        SuppressPremiumSubscriptions = 1 << 1,

        /// <summary>
        /// Suppress server setup tips.
        /// </summary>
        SuppressGuildReminderNotifications = 1 << 2,
    }
}
