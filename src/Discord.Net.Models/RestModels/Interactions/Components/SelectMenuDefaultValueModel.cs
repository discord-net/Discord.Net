using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class SelectMenuDefaultValueModel : ISelectMenuDefaultValueModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public required string Type { get; set; }
}
