#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ModifyGuildEmbedParams
    {        
        [JsonProperty("enabled")]
        internal Optional<bool> _enabled { get; set; }
        public bool Enabled { set { _enabled = value; } }

        [JsonProperty("channel")]
        internal Optional<ulong?> _channelId { get; set; }
        public ulong? ChannelId { set { _channelId = value; } }
        public IVoiceChannel Channel { set { _channelId = value != null ? value.Id : (ulong?)null; } }
    }
}
