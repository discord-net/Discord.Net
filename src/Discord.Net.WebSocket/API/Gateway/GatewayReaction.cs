using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class GatewayReaction : Reaction
    {
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
        [JsonProperty("message_id")]
        public ulong MessageId { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("emoji")]
        public Discord.API.Emoji Emoji { get; set; }
    }
}
