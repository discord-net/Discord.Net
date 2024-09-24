using Discord.Converters;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildVoice)]
public class GuildVoiceChannel : GuildChannelBase, IGuildVoiceChannelModel
{
    string? IAudioChannelModel.RTCRegion => ~RTCRegion;

    int? IAudioChannelModel.VideoQualityMode => ~VideoQualityMode;

    int IGuildVoiceChannelModel.RatelimitPerUser => ~RatelimitPerUser;

    bool IGuildVoiceChannelModel.IsNsfw => ~Nsfw;

    string? IGuildVoiceChannelModel.Topic => ~Topic;

    int IGuildVoiceChannelModel.Bitrate => ~Bitrate;

    int? IGuildVoiceChannelModel.UserLimit => ~UserLimit;
}
