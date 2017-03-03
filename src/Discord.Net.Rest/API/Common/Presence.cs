#pragma warning disable CS1591
using Newtonsoft.Json;

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
        [JsonProperty("game")]
        public Game Game { get; set; }

        [JsonProperty("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nick { get; set; }
    }
}
