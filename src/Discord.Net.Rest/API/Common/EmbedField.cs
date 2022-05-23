using Newtonsoft.Json;

namespace Discord.API
{
    internal class EmbedField : IEmbedFieldModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("inline")]
        public bool Inline { get; set; }
    }
}
