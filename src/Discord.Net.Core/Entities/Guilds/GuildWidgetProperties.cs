namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify the widget of an <see cref="IGuild" /> with the specified changes.
    /// </summary>
    public class GuildWidgetProperties
    {
        /// <summary>
        ///     Sets whether the widget should be enabled.
        /// </summary>
        public Optional<bool> Enabled { get; set; }
        /// <summary>
        ///     Sets the channel that the invite should place its users in, if not <see langword="null" />.
        /// </summary>
        public Optional<IChannel> Channel { get; set; }
        /// <summary>
        ///     Sets the channel that the invite should place its users in, if not <see langword="null" />.
        /// </summary>
        public Optional<ulong?> ChannelId { get; set; }
    }
}
