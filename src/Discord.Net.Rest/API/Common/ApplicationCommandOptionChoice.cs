using Newtonsoft.Json;

namespace Discord.API
{
    internal class ApplicationCommandOptionChoice
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
