using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the default message notification level.
    /// </summary>
    public enum DefaultMessageNotificationLevel
    {
        /// <summary>
        ///     Members will receive notifications for all messages by default.
        /// </summary>
        AllMessages = 0,

        /// <summary>
        ///     Members will receive notifications only for messages that @mention them by default.
        /// </summary>
        OnlyMentions = 1,
    }
}
