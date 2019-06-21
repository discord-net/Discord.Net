namespace Discord
{
    /// <summary>
    ///     An extension class for <see cref="IGuild"/>.
    /// </summary>
    public static class GuildExtensions
    {
        /// <summary>
        ///     Gets if welcome system messages are enabled.
        /// </summary>
        /// <param name="guild"> The guild to check. </param>
        /// <returns> A <c>bool</c> indicating if the welcome messages are enabled in the system channel. </returns>
        public static bool GetWelcomeMessagesEnabled(this IGuild guild)
            => !guild.SystemChannelFlags.HasFlag(SystemChannelMessageDeny.WelcomeMessage);

        /// <summary>
        ///     Gets if guild boost system messages are enabled.
        /// </summary>
        /// <param name="guild"> The guild to check. </param>
        /// <returns> A <c>bool</c> indicating if the guild boost messages are enabled in the system channel. </returns>
        public static bool GetGuildBoostMessagesEnabled(this IGuild guild)
            => !guild.SystemChannelFlags.HasFlag(SystemChannelMessageDeny.GuildBoost);
    }
}
