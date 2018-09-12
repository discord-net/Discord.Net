namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify an <see cref="ITextChannel"/> with the specified changes.
    /// </summary>
    /// <seealso cref="ITextChannel.ModifyAsync(System.Action{TextChannelProperties}, RequestOptions)"/>
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
        /// <summary>
        /// What the slow-mode ratelimit for this channel should be set to; 0 will disable slow-mode.
        /// </summary>
        /// <remarks>
        /// This value must fall within [0, 120]
        /// 
        /// Users with <see cref="ChannelPermission.ManageMessages"/> will be exempt from slow-mode.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Throws ArgummentOutOfRange if the value does not fall within [0, 120]</exception>
        public Optional<int> SlowModeInterval { get; set; }
    }
}
