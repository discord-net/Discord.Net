using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class IntegrationApplication
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("bot")]
    public Optional<User> Bot { get; set; }
}
