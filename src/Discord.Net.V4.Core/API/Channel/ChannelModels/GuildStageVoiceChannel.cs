using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.API;

[ChannelTypeOf(ChannelType.Stage)]
public sealed class GuildStageVoiceChannel : Channel
{
    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("last_message_id")]
    public Optional<ulong?> LastMessageId { get; set; }

    [JsonPropertyName("flags")]
    public Optional<ChannelFlags> Flags { get; set; }

    [JsonPropertyName("name")]
    public Optional<string?> Name { get; set; }

    [JsonPropertyName("parent_id")]
    public Optional<ulong?> ParentId { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int> SlowMode { get; set; }

    [JsonPropertyName("topic")]
    public Optional<string?> Topic { get; set; }

    [JsonPropertyName("position")]
    public Optional<int> Position { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public Optional<Overwrite[]> PermissionOverwrites { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool> Nsfw { get; set; }

    [JsonPropertyName("user_limit")]
    public Optional<int> UserLimit { get; set; }

    [JsonPropertyName("rtc_region")]
    public Optional<string?> RTCRegion { get; set; }

    [JsonPropertyName("video_quality_mode")]
    public Optional<VideoQualityMode> VideoQualityMode { get; set; }

    [JsonPropertyName("bitrate")]
    public Optional<int> Bitrate { get; set; }
}
