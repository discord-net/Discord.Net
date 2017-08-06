#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildSyncEvent
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("large")]
        public bool Large { get; set; }

        [ModelProperty("presences")]
        public Presence[] Presences { get; set; }
        [ModelProperty("members")]
        public GuildMember[] Members { get; set; }
    }
}
