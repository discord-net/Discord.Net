#pragma warning disable CA1721
using Newtonsoft.Json;

namespace Discord.API
{
    public class Connection
    {
        [JsonProperty("integrations")]
        public Integration[] Integrations { get; set; }
        [JsonProperty("revoked")]
        public bool Revoked { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
