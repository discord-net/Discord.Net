using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class AddGuildMemberParams
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("nick")]
    public Optional<string> Nickname { get; set; }

    [JsonPropertyName("roles")]
    public Optional<ulong[]> RoleIds { get; set; }

    [JsonPropertyName("mute")]
    public Optional<bool> IsMute { get; set; }

    [JsonPropertyName("deaf")]
    public Optional<bool> IsDeaf { get; set; }
}
