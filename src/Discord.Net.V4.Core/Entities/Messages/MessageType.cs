namespace Discord
{
    /// <summary>
    ///     Specifies the type of message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        ///     The default message type.
        /// </summary>
        Default = 0,
        /// <summary>
        ///     The message when a recipient is added.
        /// </summary>
        RecipientAdd = 1,
        /// <summary>
        ///     The message when a recipient is removed.
        /// </summary>
        RecipientRemove = 2,
        /// <summary>
        ///     The message when a user is called.
        /// </summary>
        Call = 3,
        /// <summary>
        ///     The message when a channel name is changed.
        /// </summary>
        ChannelNameChange = 4,
        /// <summary>
        ///     The message when a channel icon is changed.
        /// </summary>
        ChannelIconChange = 5,
        /// <summary>
        ///     The message when another message is pinned.
        /// </summary>
        ChannelPinnedMessage = 6,
        /// <summary>
        ///     The message when a new member joined.
        /// </summary>
        GuildMemberJoin = 7,
        /// <summary>
        ///     The message for when a user boosts a guild.
        /// </summary>
        UserPremiumGuildSubscription = 8,
        /// <summary>
        ///     The message for when a guild reaches Tier 1 of Nitro boosts.
        /// </summary>
        UserPremiumGuildSubscriptionTier1 = 9,
        /// <summary>
        ///     The message for when a guild reaches Tier 2 of Nitro boosts.
        /// </summary>
        UserPremiumGuildSubscriptionTier2 = 10,
        /// <summary>
        ///     The message for when a guild reaches Tier 3 of Nitro boosts.
        /// </summary>
        UserPremiumGuildSubscriptionTier3 = 11,
        /// <summary>
        ///     The message for when a news channel subscription is added to a text channel.
        /// </summary>
        ChannelFollowAdd = 12,
        /// <summary>
        ///     The message for when a guild is disqualified from discovery.
        /// </summary>
        GuildDiscoveryDisqualified = 14,
        /// <summary>
        ///     The message for when a guild is requalified for discovery.
        /// </summary>
        GuildDiscoveryRequalified = 15,
        /// <summary>
        ///     The message for when the initial warning is sent for the initial grace period discovery.
        /// </summary>
        GuildDiscoveryGracePeriodInitialWarning = 16,
        /// <summary>
        ///     The message for when the final warning is sent for the initial grace period discovery.
        /// </summary>
        GuildDiscoveryGracePeriodFinalWarning = 17,
        /// <summary>
        ///     The message for when a thread is created.
        /// </summary>
        ThreadCreated = 18,
        /// <summary>
        ///     The message is an inline reply.
        /// </summary>
        /// <remarks>
        ///     Only available in API v8.
        /// </remarks>
        Reply = 19,
        /// <summary>
        ///     The message is an Application Command.
        /// </summary>
        /// <remarks>
        ///     Only available in API v8.
        /// </remarks>
        ApplicationCommand = 20,
        /// <summary>
        ///     The message that starts a thread.
        /// </summary>
        /// <remarks>
        ///     Only available in API v9.
        /// </remarks>
        ThreadStarterMessage = 21,
        /// <summary>
        ///     The message for an invite reminder.
        /// </summary>
        GuildInviteReminder = 22,
        /// <summary>
        ///     The message for a context menu command.
        /// </summary>
        ContextMenuCommand = 23,
        /// <summary>
        ///     The message for an automod action.
        /// </summary>
        AutoModerationAction = 24,
        /// <summary>
        ///     The message for a role subscription purchase.
        /// </summary>
        RoleSubscriptionPurchase = 25,
        /// <summary>
        ///     The message for an interaction premium upsell.
        /// </summary>
        InteractionPremiumUpsell = 26,
        /// <summary>
        ///     The message for a stage start.
        /// </summary>
        StageStart = 27,
        /// <summary>
        ///     The message for a stage end.
        /// </summary>
        StageEnd = 28,
        /// <summary>
        ///     The message for a stage speaker.
        /// </summary>
        StageSpeaker = 29,
        /// <summary>
        ///     The message for a stage raise hand.
        /// </summary>
        StageRaiseHand = 30,
        /// <summary>
        ///     The message for a stage raise hand.
        /// </summary>
        StageTopic = 31,
        /// <summary>
        ///     The message for a guild application premium subscription.
        /// </summary>
        GuildApplicationPremiumSubscription = 32
    }
}
