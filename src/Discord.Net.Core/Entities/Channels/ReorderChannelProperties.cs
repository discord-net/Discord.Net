namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to reorder an <see cref="IGuildChannel"/>.
    /// </summary>
    public class ReorderChannelProperties
    {
        /// <summary>
        ///     Gets the ID of the channel to apply this position to.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of this channel.
        /// </returns>
        public ulong Id { get; }
        /// <summary>
        ///     Gets the new zero-based position of this channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the new position of this channel.
        /// </returns>
        public int Position { get; }

        /// <summary> Initializes a new instance of the <see cref="ReorderChannelProperties"/> class used to reorder a channel. </summary>
        /// <param name="id"> Sets the ID of the channel to apply this position to. </param>
        /// <param name="position"> Sets the new zero-based position of this channel. </param>
        public ReorderChannelProperties(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}
