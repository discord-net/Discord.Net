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
        public Optional<ThreadType> Type { get; set; }
    }
}
