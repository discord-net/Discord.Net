namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the default message notification level for a <see cref="Guild"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-object-default-message-notification-level"/>
    /// </remarks>
    public enum DefaultMessageNotificationLevel
    {
        /// <summary>
        /// <see cref="GuildMember"/>s will receive notifications for all <see cref="Message"/>s by default.
        /// </summary>
        AllMessages = 0,

        /// <summary>
        /// <see cref="GuildMember"/>s will receive notifications only for <see cref="Message"/>s that @mention them by default.
        /// </summary>
        OnlyMentions = 1,
    }
}
