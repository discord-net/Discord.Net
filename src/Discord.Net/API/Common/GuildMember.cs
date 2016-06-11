using Newtonsoft.Json;
using System;

namespace Discord.API
{
    public class GuildMember
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nick { get; set; }
        [JsonProperty("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonProperty("joined_at")]
        public Optional<DateTime> JoinedAt { get; set; }
        [JsonProperty("deaf")]
        public Optional<bool> Deaf { get; set; }
        [JsonProperty("mute")]
        public Optional<bool> Mute { get; set; }
    }
}
