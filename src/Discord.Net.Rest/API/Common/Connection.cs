using Newtonsoft.Json;
using System.Collections.Generic;

namespace Discord.API
{
    internal class Connection
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("revoked")]
        public Optional<bool> Revoked { get; set; }
        [JsonProperty("integrations")]
        public Optional<IReadOnlyCollection<Integration>> Integrations { get; set; }
        [JsonProperty("verified")]
        public bool Verified { get; set; }
        [JsonProperty("friend_sync")]
        public bool FriendSync { get; set; }
        [JsonProperty("show_activity")]
        public bool ShowActivity { get; set; }
        [JsonProperty("visibility")]
        public ConnectionVisibility Visibility { get; set; }

    }
}
