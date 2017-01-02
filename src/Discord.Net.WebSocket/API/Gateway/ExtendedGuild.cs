#pragma warning disable CS1591
using Newtonsoft.Json;
using System;

namespace Discord.API.Gateway
{
    internal class ExtendedGuild : Guild
    {
        [JsonProperty("unavailable")]
        public bool? Unavailable { get; set; }
        [JsonProperty("member_count")]
        public int MemberCount { get; set; }
        [JsonProperty("large")]
        public bool Large { get; set; }

        [JsonProperty("presences")]
        public Presence[] Presences { get; set; }
        [JsonProperty("members")]
        public GuildMember[] Members { get; set; }
        [JsonProperty("channels")]
        public Channel[] Channels { get; set; }
        [JsonProperty("joined_at")]
        public DateTimeOffset JoinedAt { get; set; }
    }
}
