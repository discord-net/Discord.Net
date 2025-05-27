using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class GuildScheduledEventUserAddRemoveEvent
    {
        [JsonProperty("guild_scheduled_event_id")]
        public ulong EventId { get; set; }
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }
        [JsonProperty("user_id")]
        public ulong UserId { get; set; }
    }
}
