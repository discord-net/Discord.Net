using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Entitlement : IEntitlementModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("sku_id")]
    public ulong SkuId { get; set; }

    [JsonPropertyName("user_id")]
    public Optional<ulong> UserId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("consumed")]
    public Optional<bool> IsConsumed { get; set; }

    [JsonPropertyName("deleted")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("starts_at")]
    public Optional<DateTimeOffset> StartsAt { get; set; }

    [JsonPropertyName("ends_at")]
    public Optional<DateTimeOffset> EndsAt { get; set; }

    ulong? IEntitlementModel.UserId => UserId.ToNullable();
    DateTimeOffset? IEntitlementModel.EndsAt => EndsAt.ToNullable();

    ulong? IEntitlementModel.GuildId => GuildId.ToNullable();

    bool? IEntitlementModel.Consumed => IsConsumed.ToNullable();
    DateTimeOffset? IEntitlementModel.StartsAt => StartsAt.ToNullable();

}
