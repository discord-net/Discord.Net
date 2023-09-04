using System;

namespace Discord
{
    /// <summary>
    ///     Represents a collection of flags regarding system channel messages.
    /// </summary>
    [Flags]
    public enum SystemChannelFlags
    {
        /// <summary>
        ///     Deny none of the system channel messages.
        ///     This will enable all of the system channel messages.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Deny the messages that are sent when a user joins the guild.
        /// </summary>
        SuppressWelcomeMessage = 1 << 0,

        /// <summary>
        ///     Deny the messages that are sent when a user boosts the guild.
        /// </summary>
        SuppressGuildBoost = 1 << 1,

        /// <summary>
        ///     Deny the messages that are related to guild setup.
        /// </summary>
        SuppressGuildSetupTip = 1 << 2,

        /// <summary>
        ///     Deny the reply with sticker button on welcome messages.
        /// </summary>
        SuppressWelcomeMessageReply = 1 << 3,


        /// <summary>
        ///     Deny role subscription purchase and renewal notifications in the guild.
        /// </summary>
        SuppressRoleSubscriptionPurchase = 1 << 4,


        /// <summary>
        ///     Hide role subscription sticker reply buttons in the guild.
        /// </summary>
        SuppressRoleSubscriptionPurchaseReplies = 1 << 5,
    }
}
