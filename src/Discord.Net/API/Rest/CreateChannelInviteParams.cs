#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class CreateChannelInviteParams
    {
        [JsonProperty("max_age")]
        internal Optional<int> _maxAge { get; set; }
        public int MaxAge { set { _maxAge = value; } }

        [JsonProperty("max_uses")]
        internal Optional<int> _maxUses { get; set; }
        public int MaxUses { set { _maxUses = value; } }

        [JsonProperty("temporary")]
        internal Optional<bool> _temporary { get; set; }
        public bool Temporary { set { _temporary = value; } }
    }
}
