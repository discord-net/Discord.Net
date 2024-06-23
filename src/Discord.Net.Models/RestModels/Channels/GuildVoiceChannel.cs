using Discord.Converters;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildVoice)]
public class GuildVoiceChannel : GuildTextChannel, IGuildVoiceChannelModel
{
    string? IAudioChannelModel.RTCRegion => RTCRegion;

    int? IAudioChannelModel.VideoQualityMode => VideoQualityMode;

    int IGuildVoiceChannelModel.Bitrate => Bitrate;

    int? IGuildVoiceChannelModel.UserLimit => UserLimit;
}
