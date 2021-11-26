using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    internal class MessageDeleteBulkEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("ids")]
        public ulong[] Ids { get; set; }
    }
}
