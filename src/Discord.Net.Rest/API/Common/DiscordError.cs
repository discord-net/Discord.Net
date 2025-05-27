using Newtonsoft.Json;

namespace Discord.API
{
    [JsonConverter(typeof(Discord.Net.Converters.DiscordErrorConverter))]
    internal class DiscordError
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("code")]
        public DiscordErrorCode Code { get; set; }
        [JsonProperty("errors")]
        public Optional<ErrorDetails[]> Errors { get; set; }
    }
}
