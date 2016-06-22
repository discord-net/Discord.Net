using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyGuildEmbedParams
    {        
        [JsonProperty("enabled")]
        public Optional<bool> Enabled { get; set; }

        [JsonProperty("channel")]
        public Optional<ulong> ChannelId { get; set; }
        [JsonIgnore]
        public Optional<IVoiceChannel> Channel { set { ChannelId = value.IsSpecified ? value.Value.Id : Optional.Create<ulong>(); } }
    }
}
