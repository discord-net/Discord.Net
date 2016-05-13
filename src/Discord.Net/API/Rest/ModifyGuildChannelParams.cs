using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildChannelParams
    {
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("position")]
        public Optional<int> Position { get; set; }
    }
}
