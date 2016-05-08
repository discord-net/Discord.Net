using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildChannelParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
    }
}
