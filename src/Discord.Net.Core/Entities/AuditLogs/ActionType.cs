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
        ///     A guild member moved to a voice channel.
        /// </summary>
        MemberMoved = 26,
        /// <summary>
        ///     A guild member disconnected from a voice channel.
        /// </summary>
        MemberDisconnected = 27,
        /// <summary>
        ///     A bot was added to this guild.
        /// </summary>
        BotAdded = 28,

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
        MessageDeleted = 72,
        /// <summary>
        ///     Multiple messages were deleted from this guild.
        /// </summary>
        MessageBulkDeleted = 73,
        /// <summary>
        ///     A message was pinned from this guild.
        /// </summary>
        MessagePinned = 74,
        /// <summary>
        ///     A message was unpinned from this guild.
        /// </summary>
        MessageUnpinned = 75,

        /// <summary>
        ///     A integration was created
        /// </summary>
        IntegrationCreated = 80,
        /// <summary>
        ///     A integration was updated
        /// </summary>
        IntegrationUpdated = 81,
        /// <summary>
        /// An integration was deleted
        /// </summary>
        IntegrationDeleted = 82,
        /// <summary>
        ///     A stage instance was created.
        /// </summary>
        StageInstanceCreated = 83,
        /// <summary>
        ///     A stage instance was updated.
        /// </summary>
        StageInstanceUpdated = 84,
        /// <summary>
        ///     A stage instance was deleted.
        /// </summary>
        StageInstanceDeleted = 85,

        /// <summary>
        ///     A sticker was created.
        /// </summary>
        StickerCreated = 90,
        /// <summary>
        ///     A sticker was updated.
        /// </summary>
        StickerUpdated = 91,
        /// <summary>
        ///     A sticker was deleted.
        /// </summary>
        StickerDeleted = 92,

        /// <summary>
        ///     A scheduled event was created.
        /// </summary>
        EventCreate = 100,
        /// <summary>
        ///     A scheduled event was created.
        /// </summary>
        EventUpdate = 101,
        /// <summary>
        ///     A scheduled event was created.
        /// </summary>
        EventDelete = 102,

        /// <summary>
        ///     A thread was created.
        /// </summary>
        ThreadCreate = 110,
        /// <summary>
        ///     A thread was updated.
        /// </summary>
        ThreadUpdate = 111,
        /// <summary>
        ///     A thread was deleted.
        /// </summary>
        ThreadDelete = 112,
        /// <summary>
        ///     Permissions were updated for a command.
        /// </summary>
        ApplicationCommandPermissionUpdate = 121,

        /// <summary>
        ///     Auto Moderation rule was created.
        /// </summary>
        AutoModerationRuleCreate = 140,
        /// <summary>
        ///     Auto Moderation rule was updated.
        /// </summary>
        AutoModerationRuleUpdate = 141,
        /// <summary>
        ///     Auto Moderation rule was deleted.
        /// </summary>
        AutoModerationRuleDelete = 142,
        /// <summary>
        ///     Message was blocked by Auto Moderation.
        /// </summary>
        AutoModerationBlockMessage = 143,
        /// <summary>
        ///     Message was flagged by Auto Moderation.
        /// </summary>
        AutoModerationFlagToChannel = 144,
        /// <summary>
        ///     Member was timed out by Auto Moderation.
        /// </summary>
        AutoModerationUserCommunicationDisabled = 145,

        /// <summary>
        ///     Guild Onboarding Question was created.
        /// </summary>
        OnboardingQuestionCreated = 163,

        /// <summary>
        ///     Guild Onboarding Question was updated.
        /// </summary>
        OnboardingQuestionUpdated = 164,

        /// <summary>
        ///     Guild Onboarding was updated.
        /// </summary>
        OnboardingUpdated = 167
    }
}
