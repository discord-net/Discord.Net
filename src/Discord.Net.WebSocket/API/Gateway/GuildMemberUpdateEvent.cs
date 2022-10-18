using System.Text.Json.Serialization;
using System;

namespace Discord.API.Gateway
{
    internal class GuildMemberUpdateEvent : GuildMember
    {
        [JsonPropertyName("joined_at")]
        public new DateTimeOffset? JoinedAt { get; set; }

        [JsonPropertyName("guild_id")]
        public ulong GuildId { get; set; }
    }
}
