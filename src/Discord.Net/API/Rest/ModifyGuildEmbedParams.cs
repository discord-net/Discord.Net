using Discord.Net.Converters;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildEmbedParams
    {        
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("channel"), JsonConverter(typeof(UInt64EntityConverter))]
        public IVoiceChannel Channel { get; set; }
    }
}
