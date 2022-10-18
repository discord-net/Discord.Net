using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyThreadParams
    {
        [JsonPropertyName("name")]
        public Optional<string> Name { get; set; }

        [JsonPropertyName("archived")]
        public Optional<bool> Archived { get; set; }

        [JsonPropertyName("auto_archive_duration")]
        public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

        [JsonPropertyName("locked")]
        public Optional<bool> Locked { get; set; }

        [JsonPropertyName("rate_limit_per_user")]
        public Optional<int> Slowmode { get; set; }
    }
}
