namespace Discord
{
    /// <inheritdoc />
    public class TextChannelProperties : GuildChannelProperties
    {
        /// <summary>
        /// What the topic of the channel should be set to.
        /// </summary>
        public Optional<string> Topic { get; set; }
        /// <summary>
        /// Should this channel be flagged as NSFW?
        /// </summary>
        public Optional<bool> IsNsfw { get; set; }
    }
}
