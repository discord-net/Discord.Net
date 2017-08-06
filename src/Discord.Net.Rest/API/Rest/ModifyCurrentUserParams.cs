#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rest
{
    internal class ModifyCurrentUserParams
    {
        [ModelProperty("username")]
        public Optional<string> Username { get; set; }
        [ModelProperty("avatar")]
        public Optional<Image?> Avatar { get; set; }
    }
}
