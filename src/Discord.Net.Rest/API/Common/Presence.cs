using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Discord.API
{
    internal class Presence
    {
        [JsonPropertyName("user")]
        public User User { get; set; }
        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonPropertyName("status")]
        public UserStatus Status { get; set; }

        [JsonPropertyName("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonPropertyName("nick")]
        public Optional<string> Nick { get; set; }
        // This property is a Dictionary where each key is the ClientType
        // and the values are the current client status.
        // The client status values are all the same.
        // Example:
        //   "client_status": { "desktop": "dnd", "mobile": "dnd" }
        [JsonPropertyName("client_status")]
        public Optional<Dictionary<string, string>> ClientStatus { get; set; }
        [JsonPropertyName("activities")]
        public List<Game> Activities { get; set; }
        [JsonPropertyName("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; set; }
    }
}
