using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    internal class GuildMemberAddEvent : GuildMember
    {
        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
    }
}
