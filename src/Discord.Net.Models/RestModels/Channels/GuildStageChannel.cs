using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildStageVoice)]
public sealed class GuildStageChannelModel : GuildVoiceChannelModel, IGuildStageChannelModel
{
    string? IAudioChannelModel.RTCRegion => RTCRegion;
    int? IAudioChannelModel.VideoQualityMode => VideoQualityMode;
}
