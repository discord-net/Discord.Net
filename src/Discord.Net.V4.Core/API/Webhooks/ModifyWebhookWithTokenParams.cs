using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyWebhookWithTokenParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("avatar")]
    public Optional<Image?> Avatar { get; set; }
}
