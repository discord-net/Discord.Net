namespace Discord.Models;

public interface IAudioChannelModel : IChannelModel
{
    string? RTCRegion { get; }
    VideoQualityMode VideoQualityMode { get; }
}
