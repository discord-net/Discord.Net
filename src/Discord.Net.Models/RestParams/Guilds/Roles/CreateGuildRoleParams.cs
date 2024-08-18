using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateGuildRoleParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("permissions")]
    public Optional<string> Permissions { get; set; }

    [JsonPropertyName("color")]
    public Optional<uint> Color { get; set; }

    [JsonPropertyName("hoist")]
    public Optional<bool> IsHoisted { get; set; }

    [JsonPropertyName("icon")]
    public Optional<string?> Icon { get; set; }

    [JsonPropertyName("unicode_emoji")]
    public Optional<string> UnicodeEmoji { get; set; }

    [JsonPropertyName("mentionable")]
    public Optional<bool> IsMentionable { get; set; }
}
