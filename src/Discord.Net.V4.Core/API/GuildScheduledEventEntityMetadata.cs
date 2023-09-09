using System.Text.Json.Serialization;

namespace Discord.API;

public class GuildScheduledEventEntityMetadata
{
    [JsonPropertyName("location")]
    public Optional<string> Location { get; set; }
}
