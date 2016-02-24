namespace Discord
{
    public interface IVoiceChannel : IChannel
    {
        int Bitrate { get; set; }
    }
}
