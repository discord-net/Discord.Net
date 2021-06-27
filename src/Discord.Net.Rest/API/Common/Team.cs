#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Team
    {
        [JsonProperty("icon")]
        public Optional<string> Icon { get; set; }
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("members")]
        public TeamMember[] TeamMembers { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("owner_user_id")]
        public ulong OwnerUserId { get; set; }
    }
}
