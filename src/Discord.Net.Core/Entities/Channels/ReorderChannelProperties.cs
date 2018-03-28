namespace Discord
{
    /// <summary> Properties that are used to reorder an <see cref="IGuildChannel"/>. </summary>
    public class ReorderChannelProperties
    {
        /// <summary> Gets the ID of the channel to apply this position to. </summary>
        public ulong Id { get; }
        /// <summary> Gets the new zero-based position of this channel. </summary>
        public int Position { get; }

        /// <summary> Creates a <see cref="ReorderChannelProperties"/> used to reorder a channel. </summary>
        /// <param name="id"> Sets the ID of the channel to apply this position to. </param>
        /// <param name="position"> Sets the new zero-based position of this channel. </param>
        public ReorderChannelProperties(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}
