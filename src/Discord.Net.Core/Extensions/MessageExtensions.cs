namespace Discord
{
    /// <summary>
    ///     Provides extension methods for <see cref="IMessage" />.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        ///     Gets a URL that jumps to the message.
        /// </summary>
        /// <param name="msg">The message to jump to.</param>
        /// <returns>
        ///     A string that contains a URL for jumping to the message in chat.
        /// </returns>
        public static string GetJumpUrl(this IMessage msg)
        {
            var channel = msg.Channel;
            return $"https://discordapp.com/channels/{(channel is IDMChannel ? "@me" : $"{(channel as ITextChannel).GuildId}")}/{channel.Id}/{msg.Id}";
        }
    }
}
