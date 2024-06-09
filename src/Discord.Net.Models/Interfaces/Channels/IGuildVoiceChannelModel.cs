namespace Discord.Models;

public interface IGuildVoiceChannelModel : IGuildTextChannelModel, IAudioChannelModel
{
    int Bitrate { get; }
    int? UserLimit { get; }
}
