using System;

namespace Discord
{
    [Flags]
    public enum SystemChannelMessageDeny
    {
        /// <summary>
        ///     Deny none of the system channel messages.
        ///     This will enable all of the system channel messages.
        /// </summary>
        None = 0,
        /// <summary>
        ///     Deny the messages that are sent when a user joins the guild.
        /// </summary>
        WelcomeMessage = 1 << 0,
        /// <summary>
        ///     Deny the messages that are sent when a user boosts the guild.
        /// </summary>
        GuildBoost = 1 << 1,
        /// <summary>
        ///     Deny the messages that are related to guild setup.
        /// </summary>
        GuildSetupTip = 1 << 2,
        /// <summary>
        ///     Deny the reply with sticker button on welcome messages.
        /// </summary>
        WelcomeMessageReply = 1 << 3,

        /// <summary>
        ///     Deny role subscription purchase and renewal notifications in the guild.
        /// </summary>
        RoleSubscriptionPurchase = 1 << 4,

        /// <summary>
        ///     Hide role subscription sticker reply buttons in the guild.
        /// </summary>
        RoleSubscriptionPurchaseReplies = 1 << 5,
    }
}
