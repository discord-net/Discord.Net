#pragma warning disable CS1591
using Discord.Serialization;
using System;

namespace Discord.API
{
    internal class GuildMember
    {
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("nick")]
        public Optional<string> Nick { get; set; }
        [ModelProperty("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [ModelProperty("joined_at")]
        public Optional<DateTimeOffset> JoinedAt { get; set; }
        [ModelProperty("deaf")]
        public Optional<bool> Deaf { get; set; }
        [ModelProperty("mute")]
        public Optional<bool> Mute { get; set; }
    }
}
