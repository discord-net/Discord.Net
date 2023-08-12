using System.Text.Json.Serialization;

namespace Discord.API;

internal class GuildScheduledEventEntityMetadata
{
    [JsonPropertyName("location")]
    public Optional<string> Location { get; set; }
}
