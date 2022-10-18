using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class GuildScheduledEventUser
    {
        [JsonPropertyName("user")]
        public User User { get; set; }
        [JsonPropertyName("member")]
        public Optional<GuildMember> Member { get; set; }
        [JsonPropertyName("guild_scheduled_event_id")]
        public ulong GuildScheduledEventId { get; set; }
    }
}
