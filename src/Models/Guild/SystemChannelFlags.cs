using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the system channel flags.
    /// </summary>
    [Flags]
    public enum SystemChannelFlags
    {
        /// <summary>
        ///     Suppress member join notifications.
        /// </summary>
        SuppressJoinNotifications = 1 << 0,

        /// <summary>
        ///     Suppress server boost notifications.
        /// </summary>
        SuppressPremiumSubscriptions = 1 << 1,

        /// <summary>
        ///     Suppress server setup tips.
        /// </summary>
        SuppressGuildReminderNotifications = 1 << 2,
    }
}
