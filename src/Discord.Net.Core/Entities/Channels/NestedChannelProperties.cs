namespace Discord
{
    /// <summary>
    ///     Properties that are used to modify an <see cref="INestedChannel"/> with the specified changes.
    /// </summary>
    public class NestedChannelProperties : GuildChannelProperties
    {
        /// <summary>
        ///     Gets or sets the category ID for this channel.
        /// </summary>
        /// <remarks>
        ///     Setting this value to a category's snowflake identifier will change or set this channel's parent to the
        ///     specified channel; setting this value to <c>null</c> will detach this channel from its parent if one
        ///     is set.
        /// </remarks>
        public Optional<ulong?> CategoryId { get; set; }
    }
}
