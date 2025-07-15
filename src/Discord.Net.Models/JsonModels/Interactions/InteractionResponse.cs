using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionResponse : IBodyParams
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("data")]
    public Optional<InteractionCallbackData> Data { get; set; }
}
