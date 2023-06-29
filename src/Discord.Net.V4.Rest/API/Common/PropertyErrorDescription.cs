using System.Text.Json.Serialization;

namespace Discord.API;

internal class ErrorDetails
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("errors")]
    public Error[] Errors { get; set; }
}
