using Discord.Converters;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildVoice)]
public class GuildVoiceChannelModel : GuildChannelModelBase, IGuildVoiceChannelModel
{
    string? IAudioChannelModel.RTCRegion => ~RTCRegion;

    int? IAudioChannelModel.VideoQualityMode => ~VideoQualityMode;

    int IGuildVoiceChannelModel.RatelimitPerUser => ~RatelimitPerUser;

    bool IGuildVoiceChannelModel.IsNsfw => ~Nsfw;

    string? IGuildVoiceChannelModel.Topic => ~Topic;

    int IGuildVoiceChannelModel.Bitrate => ~Bitrate;

    int? IGuildVoiceChannelModel.UserLimit => ~UserLimit;
}
