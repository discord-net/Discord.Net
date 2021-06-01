using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class InviteDeletedEvent
    {
        [JsonProperty("channel_id")]
        public ulong ChannelID { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildID { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
