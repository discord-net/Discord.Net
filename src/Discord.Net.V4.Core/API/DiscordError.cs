using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class DiscordError
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("code")]
    public DiscordErrorCode Code { get; set; }

    [JsonPropertyName("errors")]
    public Optional<ErrorDetails[]> Errors { get; set; }
}
