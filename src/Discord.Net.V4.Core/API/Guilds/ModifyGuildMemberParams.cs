using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyGuildMemberParams
{
    [JsonPropertyName("nick")]
    public Optional<string> Nickname { get; set; }

    [JsonPropertyName("roles")]
    public Optional<ulong[]> RoleIds { get; set; }

    [JsonPropertyName("mute")]
    public Optional<bool> IsMute { get; set; }

    [JsonPropertyName("deaf")]
    public Optional<bool> IsDeaf { get; set; }

    [JsonPropertyName("channel_id")]
    public Optional<ulong?> VoiceChannelId { get; set; }

    [JsonPropertyName("communication_disabled_until")]
    public Optional<DateTimeOffset?> CommunicationDisabledUntil { get; set; }

    [JsonPropertyName("flags")]
    public Optional<GuildUserFlags> UserFlags { get; set; }
}
