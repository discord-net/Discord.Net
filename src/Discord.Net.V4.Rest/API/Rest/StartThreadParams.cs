using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class StartThreadParams
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public ThreadArchiveDuration Duration { get; set; }

        [JsonPropertyName("type")]
        public ThreadType Type { get; set; }

        [JsonPropertyName("invitable")]
        public Optional<bool> Invitable { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int?> Ratelimit { get; set; }
    }
}
