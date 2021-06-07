namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the audit log event for an <see cref="AuditLogEntry"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/audit-log#audit-log-entry-object-audit-log-events"/>
    /// </remarks>
    public enum AuditLogEvent
    {
        /// <summary>
        /// Default value of this type.
        /// </summary>
        None = 0,

        /// <summary>
        /// The guild was updated.
        /// </summary>
        GuildUpdate = 1,

        /// <summary>
        /// A channel was created.
        /// </summary>
        ChannelCreate = 10,

        /// <summary>
        /// A channel was updated.
        /// </summary>
        ChannelUpdate = 11,

        /// <summary>
        /// A channel was deleted.
        /// </summary>
        ChannelDelete = 12,

        /// <summary>
        /// A channel overwrite was created.
        /// </summary>
        ChannelOverwriteCreate = 13,

        /// <summary>
        /// A channel overwrite was updated.
        /// </summary>
        ChannelOverwriteUpdate = 14,

        /// <summary>
        /// A channel overwrite was deleted.
        /// </summary>
        ChannelOverwriteDelete = 15,

        /// <summary>
        /// A guild member was kicked.
        /// </summary>
        MemberKick = 20,

        /// <summary>
        /// A guild member was pruned.
        /// </summary>
        MemberPrune = 21,

        /// <summary>
        /// A guild member was banned.
        /// </summary>
        MemberBanAdd = 22,

        /// <summary>
        /// A guild member was unbanned.
        /// </summary>
        MemberBanRemove = 23,

        /// <summary>
        /// A guild member was updated.
        /// </summary>
        MemberUpdate = 24,

        /// <summary>
        /// A guild role was updated.
        /// </summary>
        MemberRoleUpdate = 25,

        /// <summary>
        /// A guild member was moved.
        /// </summary>
        MemberMove = 26,

        /// <summary>
        /// A guild member was disconnected.
        /// </summary>
        MemberDisconnect = 27,

        /// <summary>
        /// A bot was added.
        /// </summary>
        BotAdd = 28,

        /// <summary>
        /// A role was created.
        /// </summary>
        RoleCreate = 30,

        /// <summary>
        /// A role was updated.
        /// </summary>
        RoleUpdate = 31,

        /// <summary>
        /// A role was deleted.
        /// </summary>
        RoleDelete = 32,

        /// <summary>
        /// An invite was created.
        /// </summary>
        InviteCreate = 40,

        /// <summary>
        /// An invite was updated.
        /// </summary>
        InviteUpdate = 41,

        /// <summary>
        /// An invite was deleted.
        /// </summary>
        InviteDelete = 42,

        /// <summary>
        /// A webhook was created.
        /// </summary>
        WebhookCreate = 50,

        /// <summary>
        /// A webhook was updated.
        /// </summary>
        WebhookUpdate = 51,

        /// <summary>
        /// A webhook was deleted.
        /// </summary>
        WebhookDelete = 52,

        /// <summary>
        /// An emoji was created.
        /// </summary>
        EmojiCreate = 60,

        /// <summary>
        /// An emoji was updated.
        /// </summary>
        EmojiUpdate = 61,

        /// <summary>
        /// An emoji was deleted.
        /// </summary>
        EmojiDelete = 62,

        /// <summary>
        /// A message was deleted.
        /// </summary>
        MessageDelete = 72,

        /// <summary>
        /// Message were deleted in bulk.
        /// </summary>
        MessageBulkDelete = 73,

        /// <summary>
        /// A message was pinned.
        /// </summary>
        MessagePin = 74,

        /// <summary>
        /// A message was unpinned.
        /// </summary>
        MessageUnpin = 75,

        /// <summary>
        /// An integration was created.
        /// </summary>
        IntegrationCreate = 80,

        /// <summary>
        /// An integration was updated.
        /// </summary>
        IntegrationUpdate = 81,

        /// <summary>
        /// An integration was deleted.
        /// </summary>
        IntegrationDelete = 82,
    }
}
