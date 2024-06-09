using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[ChannelTypeOf(ChannelType.GuildStageVoice)]
public sealed class GuildStageVoiceChannel : GuildChannelBase, IAudioChannelModel
{
    string? IAudioChannelModel.RTCRegion => RTCRegion;
    int? IAudioChannelModel.VideoQualityMode => VideoQualityMode;
}
