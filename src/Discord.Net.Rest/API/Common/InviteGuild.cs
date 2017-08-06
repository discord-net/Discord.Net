#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class InviteGuild
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("splash_hash")]
        public string SplashHash { get; set; }
    }
}
