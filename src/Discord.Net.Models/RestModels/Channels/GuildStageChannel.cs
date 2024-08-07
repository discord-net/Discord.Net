using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionType(nameof(Type), ChannelType.GuildStageVoice)]
public sealed class GuildStageChannel : GuildVoiceChannel, IGuildStageChannelModel
{
    string? IAudioChannelModel.RTCRegion => ~RTCRegion;
    int? IAudioChannelModel.VideoQualityMode => ~VideoQualityMode;
}
