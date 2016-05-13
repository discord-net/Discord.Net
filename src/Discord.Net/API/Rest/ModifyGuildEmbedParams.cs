using Discord.Net.Converters;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildEmbedParams
    {        
        [JsonProperty("enabled")]
        public Optional<bool> Enabled { get; set; }
        [JsonProperty("channel")]
        public Optional<IVoiceChannel> Channel { get; set; }
    }
}
