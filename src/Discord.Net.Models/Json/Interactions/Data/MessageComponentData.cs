using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class MessageComponentData : InteractionData, IMessageComponentDataModel
{
    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("component_type")]
    public int ComponentType { get; set; }

    [JsonPropertyName("values")]
    public Optional<string[]> Values { get; set; }

    [JsonPropertyName("resolved")]
    public Optional<InteractionDataResolved> Resolved { get; set; }

    string[]? IMessageComponentDataModel.Values => ~Values;
    IResolvedDataModel? IMessageComponentDataModel.Resolved => ~Resolved;
}
