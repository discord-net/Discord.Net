namespace Discord.Models;

public interface IGuildVoiceChannelModel : IGuildChannelModel, IAudioChannelModel
{
    int RatelimitPerUser { get; }
    bool IsNsfw { get; }
    string? Topic { get; }
    int Bitrate { get; }
    int? UserLimit { get; }
}
