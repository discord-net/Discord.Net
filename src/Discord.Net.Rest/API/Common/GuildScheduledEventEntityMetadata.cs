using Newtonsoft.Json;

namespace Discord.API
{
    internal class GuildScheduledEventEntityMetadata
    {
        [JsonProperty("location")]
        public Optional<string> Location { get; set; }
    }
}
