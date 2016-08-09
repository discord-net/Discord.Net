#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    public class EmbedProvider
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
