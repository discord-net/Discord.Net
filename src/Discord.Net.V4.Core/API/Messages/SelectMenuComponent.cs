using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class SelectMenuComponent
{
    [JsonPropertyName("type")]
    public ComponentType Type { get; set; }

    [JsonPropertyName("custom_id")]
    public required string CustomId { get; set; }

    [JsonPropertyName("options")]
    public Optional<SelectMenuOption[]> Options { get; set; }

    [JsonPropertyName("channel_types")]
    public Optional<ChannelType[]> ChannelTypes { get; set; }

    [JsonPropertyName("placeholder")]
    public Optional<string> Placeholder { get; set; }

    [JsonPropertyName("min_values")]
    public int MinValues { get; set; }

    [JsonPropertyName("max_values")]
    public int MaxValues { get; set; }

    [JsonPropertyName("disabled")]
    public bool IsDisabled { get; set; }

    [JsonPropertyName("resolved")]
    public Optional<InteractionDataResolved> Resolved { get; set; }

    [JsonPropertyName("values")]
    public Optional<string[]> Values { get; set; }
}
