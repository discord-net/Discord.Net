using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class Integration
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("enabled")]
    public bool IsEnabled { get; set; }

    [JsonPropertyName("syncing")]
    public Optional<bool> IsSyncing { get; set; }

    [JsonPropertyName("role_id")]
    public Optional<ulong> RoleId { get; set; }

    [JsonPropertyName("enable_emoticons")]
    public Optional<bool> EnableEmoticons { get; set; }

    [JsonPropertyName("expire_behavior")]
    public Optional<IntegrationExpireBehavior> ExpireBehavior { get; set; }

    [JsonPropertyName("expire_grace_period")]
    public Optional<int> ExpireGracePeriod { get; set; }

    [JsonPropertyName("user")]
    public Optional<User> User { get; set; }

    [JsonPropertyName("account")]
    public Optional<IntegrationAccount> Account { get; set; }

    [JsonPropertyName("synced_at")]
    public Optional<DateTimeOffset> SyncedAt { get; set; }

    [JsonPropertyName("subscriber_count")]
    public Optional<int> SubscriberAccount { get; set; }

    [JsonPropertyName("revoked")]
    public Optional<bool> Revoked { get; set; }

    [JsonPropertyName("application")]
    public Optional<IntegrationApplication> Application { get; set; }

    [JsonPropertyName("scopes")]
    public Optional<string[]> Scopes { get; set; }
}
