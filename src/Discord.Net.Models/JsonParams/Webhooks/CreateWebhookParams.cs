using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateWebhookParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("avatar")]
    public Optional<string?> Avatar { get; set; }
}
