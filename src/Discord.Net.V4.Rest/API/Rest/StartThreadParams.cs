using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class StartThreadParams
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public Optional<ThreadArchiveDuration> Duration { get; set; }

    [JsonPropertyName("type")]
    public Optional<ThreadType> Type { get; set; }

    [JsonPropertyName("invitable")]
    public Optional<bool> Invitable { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> Ratelimit { get; set; }
}
