using Newtonsoft.Json;

namespace Discord.API
{
    internal class IntegrationAccount
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
