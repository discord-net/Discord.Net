using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class CreateWebhookParams
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("avatar")]
    public Optional<Image?> Avatar { get; set; }
}
