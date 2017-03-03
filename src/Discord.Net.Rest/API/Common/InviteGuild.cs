#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class InviteGuild
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("splash_hash")]
        public string SplashHash { get; set; }
    }
}
