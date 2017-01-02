#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class GetChannelParams
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
    }
}
