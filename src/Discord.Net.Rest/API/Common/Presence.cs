#pragma warning disable CS1591
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Discord.API
{
    internal class Presence
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("status")]
        public UserStatus Status { get; set; }

        [JsonProperty("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nick { get; set; }
        // This property is a Dictionary where each key is the ClientType
        // and the values are the current client status.
        // The client status values are all the same.
        // Example:
        //   "client_status": { "desktop": "dnd", "mobile": "dnd" }
        [JsonProperty("client_status")]
        public Optional<Dictionary<string, string>> ClientStatus { get; set; }
        [JsonProperty("activities")]
        public List<Game> Activities { get; set; }
        [JsonProperty("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; set; }
    }
}
