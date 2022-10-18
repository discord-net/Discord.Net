using System.Text.Json.Serialization;
using System;

namespace Discord.API.Gateway
{
    internal class ExtendedGuild : Guild
    {
        [JsonPropertyName("unavailable")]
        public bool? Unavailable { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }

        [JsonPropertyName("large")]
        public bool Large { get; set; }

        [JsonPropertyName("presences")]
        public Presence[] Presences { get; set; }

        [JsonPropertyName("members")]
        public GuildMember[] Members { get; set; }

        [JsonPropertyName("channels")]
        public Channel[] Channels { get; set; }

        [JsonPropertyName("joined_at")]
        public DateTimeOffset JoinedAt { get; set; }

        [JsonPropertyName("threads")]
        public new Channel[] Threads { get; set; }

        [JsonPropertyName("guild_scheduled_events")]
        public GuildScheduledEvent[] GuildScheduledEvents { get; set; }
    }
}
