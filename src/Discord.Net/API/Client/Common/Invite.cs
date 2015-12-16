using Newtonsoft.Json;
using System;

namespace Discord.API.Client
{
    public class Invite : InviteReference
    {
        [JsonProperty("max_age")]
        public int? MaxAge { get; set; }
        [JsonProperty("max_uses")]
        public int? MaxUses { get; set; }
        [JsonProperty("revoked")]
        public bool? IsRevoked { get; set; }
        [JsonProperty("temporary")]
        public bool? IsTemporary { get; set; }
        [JsonProperty("uses")]
        public int? Uses { get; set; }
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonProperty("inviter")]
        public UserReference Inviter { get; set; }
    }
}
