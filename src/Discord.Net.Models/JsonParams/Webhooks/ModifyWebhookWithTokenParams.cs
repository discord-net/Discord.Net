using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyWebhookWithTokenParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("avatar")]
    public Optional<string?> Avatar { get; set; }
}
