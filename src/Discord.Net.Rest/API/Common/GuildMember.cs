using System.Text.Json.Serialization;
using System;

namespace Discord.API
{
    internal class GuildMember
    {
        [JsonPropertyName("user")]
        public User User { get; set; }
        [JsonPropertyName("nick")]
        public Optional<string> Nick { get; set; }
        [JsonPropertyName("avatar")]
        public Optional<string> Avatar { get; set; }
        [JsonPropertyName("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonPropertyName("joined_at")]
        public Optional<DateTimeOffset> JoinedAt { get; set; }
        [JsonPropertyName("deaf")]
        public Optional<bool> Deaf { get; set; }
        [JsonPropertyName("mute")]
        public Optional<bool> Mute { get; set; }
        [JsonPropertyName("pending")]
        public Optional<bool> Pending { get; set; }
        [JsonPropertyName("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; set; }
        [JsonPropertyName("communication_disabled_until")]
        public Optional<DateTimeOffset?> TimedOutUntil { get; set; }
    }
}
