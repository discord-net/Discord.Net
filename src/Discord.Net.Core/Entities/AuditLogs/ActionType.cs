namespace Discord
{
    /// <summary>
    ///     Representing a type of action within an <see cref="IAuditLogEntry"/>.
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        ///     this guild was updated.
        /// </summary>
        GuildUpdated = 1,

        /// <summary>
        ///     A channel was created.
        /// </summary>
        ChannelCreated = 10,
        /// <summary>
        ///     A channel was updated.
        /// </summary>
        ChannelUpdated = 11,
        /// <summary>
        ///     A channel was deleted.
        /// </summary>
        ChannelDeleted = 12,

        /// <summary>
        ///     A permission overwrite was created for a channel.
        /// </summary>
        OverwriteCreated = 13,
        /// <summary>
        ///     A permission overwrite was updated for a channel.
        /// </summary>
        OverwriteUpdated = 14,
        /// <summary>
        ///     A permission overwrite was deleted for a channel.
        /// </summary>
        OverwriteDeleted = 15,

        /// <summary>
        ///     A user was kicked from this guild.
        /// </summary>
        Kick = 20,
        /// <summary>
        ///     A prune took place in this guild.
        /// </summary>
        Prune = 21,
        /// <summary>
        ///     A user banned another user from this guild.
        /// </summary>
        Ban = 22,
        /// <summary>
        ///     A user unbanned another user from this guild.
        /// </summary>
        Unban = 23,

        /// <summary>
        ///     A guild member whose information was updated.
        /// </summary>
        MemberUpdated = 24,
        /// <summary>
        ///     A guild member's role collection was updated.
        /// </summary>
        MemberRoleUpdated = 25,

        /// <summary>
        ///     A role was created in this guild.
        /// </summary>
        RoleCreated = 30,
        /// <summary>
        ///     A role was updated in this guild.
        /// </summary>
        RoleUpdated = 31,
        /// <summary>
        ///     A role was deleted from this guild.
        /// </summary>
        RoleDeleted = 32,

        /// <summary>
        ///     An invite was created in this guild.
        /// </summary>
        InviteCreated = 40,
        /// <summary>
        ///     An invite was updated in this guild.
        /// </summary>
        InviteUpdated = 41,
        /// <summary>
        ///     An invite was deleted from this guild.
        /// </summary>
        InviteDeleted = 42,

        /// <summary>
        ///     A Webhook was created in this guild.
        /// </summary>
        WebhookCreated = 50,
        /// <summary>
        ///     A Webhook was updated in this guild.
        /// </summary>
        WebhookUpdated = 51,
        /// <summary>
        ///     A Webhook was deleted from this guild.
        /// </summary>
        WebhookDeleted = 52,

        /// <summary>
        ///     An emoji was created in this guild.
        /// </summary>
        EmojiCreated = 60,
        /// <summary>
        ///     An emoji was updated in this guild.
        /// </summary>
        EmojiUpdated = 61,
        /// <summary>
        ///     An emoji was deleted from this guild.
        /// </summary>
        EmojiDeleted = 62,

        /// <summary>
        ///     A message was deleted from this guild.
        /// </summary>
        MessageDeleted = 72
    }
}
