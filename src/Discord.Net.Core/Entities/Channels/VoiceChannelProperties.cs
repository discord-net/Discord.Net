namespace Discord
{
    /// <inheritdoc />
    public class VoiceChannelProperties : GuildChannelProperties
    {
        /// <summary>
        /// The bitrate of the voice connections in this channel. Must be greater than 8000
        /// </summary>
        public Optional<int> Bitrate { get; set; }
        /// <summary>
        /// The maximum number of users that can be present in a channel.
        /// </summary>
        public Optional<int?> UserLimit { get; set; }
    }
}
