namespace Discord.Models;

[ModelEquality]
public partial interface IGuildVoiceChannelModel : IGuildChannelModel, IAudioChannelModel
{
    int RatelimitPerUser { get; }
    bool IsNsfw { get; }
    string? Topic { get; }
    int Bitrate { get; }
    int? UserLimit { get; }
}
