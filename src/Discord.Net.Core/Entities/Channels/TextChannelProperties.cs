namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify an <see cref="ITextChannel"/> with the specified changes.
    /// </summary>
    public class TextChannelProperties : GuildChannelProperties
    {
        /// <summary>
        ///     Gets or sets the topic of the channel.
        /// </summary>
        public Optional<string> Topic { get; set; }
        /// <summary>
        ///     Gets or sets whether this channel should be flagged as NSFW.
        /// </summary>
        public Optional<bool> IsNsfw { get; set; }
    }
}
