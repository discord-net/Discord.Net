using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[InteractionDataType(InteractionDataTypes.MessageComponent)]
public class MessageComponentData : InteractionData
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("component_type")]
    public int ComponentType { get; set; }

    [JsonPropertyName("values")]
    public Optional<SelectMenuOption[]> Values { get; set; }

    [JsonPropertyName("resolved")]
    public Optional<InteractionDataResolved> Resolved { get; set; }
}
