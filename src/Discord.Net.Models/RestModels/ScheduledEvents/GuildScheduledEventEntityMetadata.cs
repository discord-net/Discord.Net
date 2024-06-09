using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildScheduledEventEntityMetadata
{
    [JsonPropertyName("location")]
    public Optional<string> Location { get; set; }
}
