#pragma warning disable CS1591
using Discord.Serialization;
using System;

namespace Discord.API
{
    internal class Integration
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("type")]
        public string Type { get; set; }
        [ModelProperty("enabled")]
        public bool Enabled { get; set; }
        [ModelProperty("syncing")]
        public bool Syncing { get; set; }
        [ModelProperty("role_id")]
        public ulong RoleId { get; set; }
        [ModelProperty("expire_behavior")]
        public ulong ExpireBehavior { get; set; }
        [ModelProperty("expire_grace_period")]
        public ulong ExpireGracePeriod { get; set; }
        [ModelProperty("user")]
        public User User { get; set; }
        [ModelProperty("account")]
        public IntegrationAccount Account { get; set; }
        [ModelProperty("synced_at")]
        public DateTimeOffset SyncedAt { get; set; }
    }
}
