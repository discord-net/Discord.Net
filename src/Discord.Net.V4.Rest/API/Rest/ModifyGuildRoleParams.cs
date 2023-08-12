using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildRoleParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("permissions")]
    public Optional<string> Permissions { get; set; }

    [JsonPropertyName("color")]
    public Optional<uint> Color { get; set; }

    [JsonPropertyName("hoist")]
    public Optional<bool> Hoist { get; set; }

    [JsonPropertyName("icon")]
    public Optional<Image?> Icon { get; set; }

    [JsonPropertyName("unicode_emoji")]
    public Optional<string> Emoji { get; set; }

    [JsonPropertyName("mentionable")]
    public Optional<bool> IsMentionable { get; set; }
}
