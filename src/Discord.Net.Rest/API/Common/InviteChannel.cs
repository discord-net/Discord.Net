using Newtonsoft.Json;

namespace Discord.API
{
    internal class InviteChannel
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}
