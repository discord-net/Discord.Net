using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class ThreadMember
    {
        [JsonProperty("id")]
        public Optional<ulong> Id { get; set; }

        [JsonProperty("user_id")]
        public Optional<ulong> UserId { get; set; }

        [JsonProperty("join_timestamp")]
        public DateTimeOffset JoinTimestamp { get; set; }

        [JsonProperty("presence")]
        public Optional<Presence> Presence { get; set; }

        [JsonProperty("member")]
        public Optional<GuildMember> Member { get; set; }

        [JsonProperty("flags")]
        public int Flags { get; set; } // No enum type (yet?)
    }
}
