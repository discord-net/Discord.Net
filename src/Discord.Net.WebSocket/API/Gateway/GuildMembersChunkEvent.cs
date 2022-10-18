using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildMembersChunkEvent
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
        [JsonPropertyName("members")]
        public GuildMember[] Members { get; set; }
    }
}
