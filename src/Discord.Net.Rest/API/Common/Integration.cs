using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class Integration
    {
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }

        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("syncing")]
        public Optional<bool?> Syncing { get; set; }
        [JsonProperty("role_id")]
        public Optional<ulong?> RoleId { get; set; }
        [JsonProperty("enable_emoticons")]
        public Optional<bool?> EnableEmoticons { get; set; }
        [JsonProperty("expire_behavior")]
        public Optional<IntegrationExpireBehavior> ExpireBehavior { get; set; }
        [JsonProperty("expire_grace_period")]
        public Optional<int?> ExpireGracePeriod { get; set; }
        [JsonProperty("user")]
        public Optional<User> User { get; set; }
        [JsonProperty("account")]
        public Optional<IntegrationAccount> Account { get; set; }
        [JsonProperty("synced_at")]
        public Optional<DateTimeOffset> SyncedAt { get; set; }
        [JsonProperty("subscriber_count")]
        public Optional<int?> SubscriberAccount { get; set; }
        [JsonProperty("revoked")]
        public Optional<bool?> Revoked { get; set; }
        [JsonProperty("application")]
        public Optional<IntegrationApplication> Application { get; set; }
    }
}
