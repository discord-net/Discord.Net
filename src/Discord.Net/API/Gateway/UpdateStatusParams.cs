using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class UpdateStatusCommand
    {
        [JsonProperty("idle_since")]
        public long? IdleSince { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
    }
}
