using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class GuildSummary
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
