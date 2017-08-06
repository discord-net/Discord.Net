#pragma warning disable CS1591
using Discord.Serialization;
using System;

namespace Discord.API.Gateway
{
    internal class ExtendedGuild : Guild
    {
        [ModelProperty("unavailable")]
        public bool? Unavailable { get; set; }
        [ModelProperty("member_count")]
        public int MemberCount { get; set; }
        [ModelProperty("large")]
        public bool Large { get; set; }

        [ModelProperty("presences")]
        public Presence[] Presences { get; set; }
        [ModelProperty("members")]
        public GuildMember[] Members { get; set; }
        [ModelProperty("channels")]
        public Channel[] Channels { get; set; }
        [ModelProperty("joined_at")]
        public DateTimeOffset JoinedAt { get; set; }
    }
}
