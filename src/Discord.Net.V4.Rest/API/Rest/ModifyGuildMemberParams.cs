using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildMemberParams
{
    [JsonPropertyName("nick")]
    public Optional<string> Nickname { get; set; }

    [JsonPropertyName("roles")]
    public Optional<ulong[]> RoleIds { get; set; }

    [JsonPropertyName("mute")]
    public Optional<bool> IsMuted { get; set; }

    [JsonPropertyName("deaf")]
    public Optional<bool> IsDeaf { get; set; }

    [JsonPropertyName("channel_id")]
    public Optional<ulong?> ChannelId { get; set; }

    [JsonPropertyName("communication_disabled_until")]
    public Optional<DateTimeOffset?> TimedOutUntil { get; set; }

    [JsonPropertyName("flags")]
    public Optional<GuildUserFlags> Flags { get; set; }
}
