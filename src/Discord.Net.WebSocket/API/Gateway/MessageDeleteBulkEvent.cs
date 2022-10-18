using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    internal class MessageDeleteBulkEvent
    {
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonPropertyName("ids")]
        public ulong[] Ids { get; set; }
    }
}
