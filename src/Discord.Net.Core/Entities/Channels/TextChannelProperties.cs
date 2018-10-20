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
        ///     Gets or sets the slow-mode ratelimit in seconds for this channel.
        /// </summary>
        /// <remarks>
        ///     Setting this value to <c>0</c> will disable slow-mode for this channel.
        ///     <note>
        ///         Users with <see cref="Discord.ChannelPermission.ManageMessages" /> will be exempt from slow-mode.
        ///     </note>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the value does not fall within [0, 120].</exception>
        public Optional<int> SlowModeInterval { get; set; }
    }
}
