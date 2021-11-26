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
        WelcomeMessage = 0b1,
        /// <summary>
        ///     Deny the messages that are sent when a user boosts the guild.
        /// </summary>
        GuildBoost = 0b10,
        /// <summary>
        ///     Deny the messages that are related to guild setup.
        /// </summary>
        GuildSetupTip = 0b100,
        /// <summary>
        ///     Deny the reply with sticker button on welcome messages.
        /// </summary>
        WelcomeMessageReply = 0b1000
    }
}
