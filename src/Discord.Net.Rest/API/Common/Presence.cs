#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Presence
    {
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [ModelProperty("status")]
        [ModelStringEnum]
        public UserStatus Status { get; set; }
        [ModelProperty("game")]
        public Game Game { get; set; }

        [ModelProperty("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [ModelProperty("nick")]
        public Optional<string> Nick { get; set; }
    }
}
