using Newtonsoft.Json;

namespace Discord.API
{
    internal class Emoji
    {
        [JsonProperty("id")]
        public ulong? Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("animated")]
        public bool? Animated { get; set; }
        [JsonProperty("roles")]
        public ulong[] Roles { get; set; }
        [JsonProperty("require_colons")]
        public bool RequireColons { get; set; }
        [JsonProperty("managed")]
        public bool Managed { get; set; }
        [JsonProperty("user")]
        public Optional<User> User { get; set; }
    }
}
