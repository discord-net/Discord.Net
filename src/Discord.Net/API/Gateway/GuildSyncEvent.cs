#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    public class GuildSyncEvent
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("large")]
        public bool Large { get; set; }

        [JsonProperty("presences")]
        public Presence[] Presences { get; set; }
        [JsonProperty("members")]
        public GuildMember[] Members { get; set; }
    }
}
