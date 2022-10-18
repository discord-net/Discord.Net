using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal class GuildScheduledEventUserAddRemoveEvent
    {
        [JsonPropertyName("guild_scheduled_event_id")]
        public ulong EventId { get; set; }
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("user_id")]
        public ulong UserId { get; set; }
    }
}
