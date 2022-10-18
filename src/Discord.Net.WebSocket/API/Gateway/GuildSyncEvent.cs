using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildSyncEvent
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("large")]
        public bool Large { get; set; }

        [JsonPropertyName("presences")]
        public Presence[] Presences { get; set; }
        [JsonPropertyName("members")]
        public GuildMember[] Members { get; set; }
    }
}
