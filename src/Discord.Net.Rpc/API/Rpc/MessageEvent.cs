#pragma warning disable CS1591
using Newtonsoft.Json;
namespace Discord.API.Rpc
{
    public class MessageEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("message")]
        public Message Message { get; set; }
    }
}
