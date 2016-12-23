namespace Discord
{
    public class BulkGuildChannelProperties
    {
        /// <summary>
        /// The id of the channel to apply this position to.
        /// </summary>
        public ulong Id { get; set; }
        /// <summary>
        /// The new zero-based position of this channel.
        /// </summary>
        public int Position { get; set; }

        public BulkGuildChannelProperties(ulong id, int position)
        {
            Id = id;
            Position = position;
        }
    }
}
