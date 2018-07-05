#pragma warning disable CS1591
using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class InviteMetadata : Invite
    {
        [JsonProperty("inviter")]
        public User Inviter { get; set; }
        [JsonProperty("uses")]
        public Optional<int> Uses { get; set; }
        [JsonProperty("max_uses")]
        public Optional<int> MaxUses { get; set; }
        [JsonProperty("max_age")]
        public Optional<int> MaxAge { get; set; }
        [JsonProperty("temporary")]
        public bool Temporary { get; set; }
        [JsonProperty("created_at")]
        public Optional<DateTimeOffset> CreatedAt { get; set; }
        [JsonProperty("revoked")]
        public bool Revoked { get; set; }
    }
}
