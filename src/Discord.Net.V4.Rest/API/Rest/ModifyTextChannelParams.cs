using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyTextChannelParams : ModifyGuildChannelParams
{
    [JsonPropertyName("topic")]
    public Optional<string> Topic { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int> SlowModeInterval { get; set; }

    [JsonPropertyName("default_thread_rate_limit_per_user")]
    public Optional<int> DefaultSlowModeInterval { get; set; }

    [JsonPropertyName("default_auto_archive_duration")]
    public Optional<ThreadArchiveDuration> DefaultAutoArchiveDuration { get; set; }
}
