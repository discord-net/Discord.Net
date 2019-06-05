using System;

namespace Discord
{
    [Flags]
    public enum SystemChannelMessageDeny
    {
        /// <summary>
        ///     The messages that are sent when a user joins the guild.
        /// </summary>
        WelcomeMessage = 0b1,
        /// <summary>
        ///     The messages that are sent when a user boosts the guild.
        /// </summary>
        GuildBoost = 0b10
    }
}
