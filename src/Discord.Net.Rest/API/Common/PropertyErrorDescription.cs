using Newtonsoft.Json;

namespace Discord.API
{
    internal class ErrorDetails
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("errors")]
        public Error[] Errors { get; set; }
    }
}
