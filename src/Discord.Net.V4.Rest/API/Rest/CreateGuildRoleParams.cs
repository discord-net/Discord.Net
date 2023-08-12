using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreateGuildRoleParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("permissions")]
    public GuildPermission Permissions { get; set; }

    [JsonPropertyName("color")]
    public uint Color { get; set; }

    [JsonPropertyName("hoist")]
    public bool Hoist { get; set; }

    [JsonPropertyName("icon")]
    public Optional<string?> Icon { get; set; }

    [JsonPropertyName("unicode_emoji")]
    public Optional<string?> Emoji { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("mentionable")]
    public bool IsMentionable { get; set; }
}
