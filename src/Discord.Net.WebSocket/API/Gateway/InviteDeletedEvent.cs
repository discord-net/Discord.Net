using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class InviteDeletedEvent
    {
        [JsonPropertyName("channel_id")]
        public ulong ChannelID { get; set; }
        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildID { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
