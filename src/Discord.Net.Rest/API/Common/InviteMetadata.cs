using System.Text.Json.Serialization;
using System;

namespace Discord.API
{
    internal class InviteMetadata : Invite
    {
        [JsonPropertyName("uses")]
        public int Uses { get; set; }
        [JsonPropertyName("max_uses")]
        public int MaxUses { get; set; }
        [JsonPropertyName("max_age")]
        public int MaxAge { get; set; }
        [JsonPropertyName("temporary")]
        public bool Temporary { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
