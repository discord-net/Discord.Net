using Newtonsoft.Json;

namespace Discord.API
{
    internal class GuildScheduledEventUser
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("member")]
        public Optional<GuildMember> Member { get; set; }
        [JsonProperty("guild_scheduled_event_id")]
        public ulong GuildScheduledEventId { get; set; }
    }
}
