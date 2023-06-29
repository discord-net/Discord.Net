using System.Text.Json.Serialization;
using Discord.Rest.Converters;

namespace Discord.API;

[JsonConverter<DiscordErrorConverter>]
internal class DiscordError
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("code")]
    public DiscordErrorCode Code { get; set; }

    [JsonPropertyName("errors")]
    public Optional<ErrorDetails[]> Errors { get; set; }
}
