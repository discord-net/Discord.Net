namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the message type for a <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/channel#message-object-message-types"/>
    /// </remarks>
    public enum MessageType
    {
        /// <summary>
        /// Default message type.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Recipient was added to a group <see cref="Channel"/>.
        /// </summary>
        RecipientAdd = 1,

        /// <summary>
        /// Recipient was removed from a group <see cref="Channel"/>.
        /// </summary>
        RecipientRemove = 2,

        /// <summary>
        /// Request to join a call.
        /// </summary>
        Call = 3,

        /// <summary>
        /// Group <see cref="Channel"/> name was changed.
        /// </summary>
        ChannelNameChange = 4,

        /// <summary>
        /// Group <see cref="Channel"/> icon was changed.
        /// </summary>
        ChannelIconChange = 5,

        /// <summary>
        /// <see cref="Message"/> was pinned in this <see cref="Channel"/>.
        /// </summary>
        ChannelPinnedMessage = 6,

        /// <summary>
        /// <see cref="User"/> joined the <see cref="Guild"/>.
        /// </summary>
        GuildMemberJoin = 7,

        /// <summary>
        /// A <see cref="GuildMember"/> boosted the <see cref="Guild"/>.
        /// </summary>
        UserPremiumGuildSubscription = 8,

        /// <summary>
        /// <see cref="Guild"/> reached tier one of nitro boost.
        /// </summary>
        UserPremiumGuildSubscriptionTier1 = 9,

        /// <summary>
        /// <see cref="Guild"/> reached tier two of nitro boost.
        /// </summary>
        UserPremiumGuildSubscriptionTier2 = 10,

        /// <summary>
        /// <see cref="Guild"/> reached tier three of nitro boost.
        /// </summary>
        UserPremiumGuildSubscriptionTier3 = 11,

        /// <summary>
        /// <see cref="Channel"/> has been followed (for news channels).
        /// </summary>
        ChannelFollowAdd = 12,

        /// <summary>
        /// <see cref="Guild"/> removed from guild discovery.
        /// </summary>
        GuildDiscoveryDisqualified = 14,

        /// <summary>
        /// <see cref="Guild"/> requalified to guild discovery.
        /// </summary>
        GuildDiscoveryRequalified = 15,

        /// <summary>
        /// First warning that the <see cref="Guild"/> failed to reach
        /// guild discovery requirements.
        /// </summary>
        GuildDiscoveryGracePeriodInitialWarning = 16,

        /// <summary>
        /// Last warning that the <see cref="Guild"/> failed to reach
        /// guild discovery requirements.
        /// </summary>
        GuildDiscoveryGracePeriodFinalWarning = 17,

        /// <summary>
        /// A thread <see cref="Channel"/> was created.
        /// </summary>
        ThreadCreated = 18,

        /// <summary>
        /// This <see cref="Message"/> is a reply.
        /// </summary>
        Reply = 19,

        /// <summary>
        /// An application command was executed.
        /// </summary>
        ApplicationCommand = 20,

        /// <summary>
        /// Starter message for a thread <see cref="Channel"/>.
        /// </summary>
        ThreadStarterMessage = 21,

        /// <summary>
        /// Reminder to invite people to the <see cref="Guild"/>.
        /// </summary>
        GuildInviteReminder = 22,
    }
}
