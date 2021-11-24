using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class ModifyThreadParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }

        [JsonProperty("archived")]
        public Optional<bool> Archived { get; set; }

        [JsonProperty("auto_archive_duration")]
        public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

        [JsonProperty("locked")]
        public Optional<bool> Locked { get; set; }

        [JsonProperty("rate_limit_per_user")]
        public Optional<int> Slowmode { get; set; }
    }
}
