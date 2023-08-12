namespace Discord.Models;

public interface IAudioChannelModel : IChannel
{
    string? RTCRegion { get; }
    VideoQualityMode VideoQualityMode { get; }
}
