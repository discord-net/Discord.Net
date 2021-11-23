using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class StartThreadParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("auto_archive_duration")]
        public ThreadArchiveDuration Duration { get; set; }

        [JsonProperty("type")]
        public ThreadType Type { get; set; }

        [JsonProperty("invitable")]
        public Optional<bool> Invitable { get; set; }

        [JsonProperty("rate_limit_per_user")]
        public Optional<int?> Ratelimit { get; set; }
    }
}
