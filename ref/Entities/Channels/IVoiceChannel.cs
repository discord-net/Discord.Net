namespace Discord
{
    public interface IVoiceChannel : IChannel
    {
        /// <summary> Gets the requested bitrate, in bits per second, of this voice channel. </summary>
        int Bitrate { get; }
    }
}
