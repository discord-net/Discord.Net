using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class GuildCreatedEvent
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
