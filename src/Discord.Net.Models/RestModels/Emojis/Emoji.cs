using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Emoji
{
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("roles")]
    public Optional<ulong[]> RoleIds { get; set; }

    [JsonPropertyName("user")]
    public Optional<User> User { get; set; }

    [JsonPropertyName("require_colons")]
    public Optional<bool> RequireColons { get; set; }

    [JsonPropertyName("managed")]
    public Optional<bool> IsManaged { get; set; }

    [JsonPropertyName("animated")]
    public Optional<bool> IsAnimated { get; set; }

    [JsonPropertyName("available")]
    public Optional<bool> IsAvailable { get; set; }
}
