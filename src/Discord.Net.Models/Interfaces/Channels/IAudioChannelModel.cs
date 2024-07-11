namespace Discord.Models;

[ModelEquality]
public partial interface IAudioChannelModel : IChannelModel
{
    string? RTCRegion { get; }
    int? VideoQualityMode { get; }
}
