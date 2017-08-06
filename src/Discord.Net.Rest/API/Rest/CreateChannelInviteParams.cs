#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class CreateChannelInviteParams
    {
        [ModelProperty("max_age")]
        public Optional<int> MaxAge { get; set; }
        [ModelProperty("max_uses")]
        public Optional<int> MaxUses { get; set; }
        [ModelProperty("temporary")]
        public Optional<bool> IsTemporary { get; set; }
        [ModelProperty("unique")]
        public Optional<bool> IsUnique { get; set; }
    }
}
