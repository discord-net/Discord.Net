using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the message type.
    /// </summary>
    [Flags]
    public enum MessageType
    {
        /// <summary>
        ///     Default message type.
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Recipient was added.
        /// </summary>
        RecipientAdd = 1,

        /// <summary>
        ///     Recipient was removed.
        /// </summary>
        RecipientRemove = 2,

        /// <summary>
        ///     Request to join a call.
        /// </summary>
        Call = 3,

        /// <summary>
        ///     Channel name was changed.
        /// </summary>
        ChannelNameChange = 4,

        /// <summary>
        ///     Channel icon was changed.
        /// </summary>
        ChannelIconChange = 5,

        /// <summary>
        ///     Message was pinned in this channel.
        /// </summary>
        ChannelPinnedMessage = 6,

        /// <summary>
        ///     User joined the guild.
        /// </summary>
        GuildMemberJoin = 7,

        /// <summary>
        ///     
        /// </summary>
        UserPremiumGuildSubscription = 8,

        /// <summary>
        ///     
        /// </summary>
        UserPremiumGuildSubscriptionTier1 = 9,

        /// <summary>
        ///     
        /// </summary>
        UserPremiumGuildSubscriptionTier2 = 10,

        /// <summary>
        ///     
        /// </summary>
        UserPremiumGuildSubscriptionTier3 = 11,

        /// <summary>
        ///     
        /// </summary>
        ChannelFollowAdd = 12,

        /// <summary>
        ///     
        /// </summary>
        GuildDiscoveryDisqualified = 14,

        /// <summary>
        ///     
        /// </summary>
        GuildDiscoveryRequalified = 15,

        /// <summary>
        ///     
        /// </summary>
        GuildDiscoveryGracePeriodInitialWarning = 16,

        /// <summary>
        ///     
        /// </summary>
        GuildDiscoveryGracePeriodFinalWarning = 17,

        /// <summary>
        ///     
        /// </summary>
        ThreadCreated = 18,

        /// <summary>
        ///     
        /// </summary>
        Reply = 19,

        /// <summary>
        ///     
        /// </summary>
        ApplicationCommand = 20,

        /// <summary>
        ///     
        /// </summary>
        ThreadStarterMessage = 21,

        /// <summary>
        ///     
        /// </summary>
        GuildInviteReminder = 22,
    }
}
