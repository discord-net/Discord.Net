namespace Discord
{
    /// <summary>
    /// Modify the widget of an IGuild with the specified parameters
    /// </summary>
    public class GuildEmbedProperties
    {
        /// <summary>
        /// Should the widget be enabled?
        /// </summary>
        public Optional<bool> Enabled { get; set; }
        /// <summary>
        /// What channel should the invite place users in, if not null.
        /// </summary>
        public Optional<IChannel> Channel { get; set; }
        /// <summary>
        /// What channel should the invite place users in, if not null.
        /// </summary>
        public Optional<ulong?> ChannelId { get; set; }
    }
}
