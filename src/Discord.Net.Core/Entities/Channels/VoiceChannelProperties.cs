namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify an <see cref="IVoiceChannel" /> with the specified changes.
    /// </summary>
    public class VoiceChannelProperties : GuildChannelProperties
    {
        /// <summary>
        ///     Gets or sets the bitrate of the voice connections in this channel. Must be greater than 8000.
        /// </summary>
        public Optional<int> Bitrate { get; set; }
        /// <summary>
        ///     Gets or sets the maximum number of users that can be present in a channel, or <c>null</c> if none.
        /// </summary>
        public Optional<int?> UserLimit { get; set; }
    }
}
