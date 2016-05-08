using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildChannelsParams
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("position")]
        public int Position { get; set; }
    }
}
