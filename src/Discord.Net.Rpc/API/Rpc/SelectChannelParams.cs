#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class SelectChannelParams
    {
        [JsonProperty("channel_id")]
        public ulong? ChannelId { get; set; }
        [JsonProperty("force")]
        public Optional<bool> Force { get; set; }
    }
}
