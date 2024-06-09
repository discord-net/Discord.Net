namespace Discord.Models;

public interface IAudioChannelModel : IChannelModel
{
    string? RTCRegion { get; }
    int? VideoQualityMode { get; }
}
