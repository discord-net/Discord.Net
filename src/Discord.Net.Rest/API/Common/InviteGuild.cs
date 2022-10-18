using System.Text.Json.Serialization;

namespace Discord.API
{
    internal class InviteGuild
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("splash_hash")]
        public string SplashHash { get; set; }
    }
}
