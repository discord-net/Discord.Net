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

        [JsonProperty("flags")]
        public int Flags { get; set; } // No enum type (yet?)

        [JsonProperty("member")]
        public Optional<GuildMember> GuildMember { get; set; }
    }
}
