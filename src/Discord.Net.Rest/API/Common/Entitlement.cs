using Newtonsoft.Json;
using System;

namespace Discord.API;

internal class Entitlement
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("sku_id")]
    public ulong SkuId { get; set; }

    [JsonProperty("user_id")]
    public Optional<ulong> UserId { get; set; }

    [JsonProperty("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonProperty("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonProperty("type")]
    public EntitlementType Type { get; set; }

    [JsonProperty("consumed")]
    public Optional<bool> IsConsumed { get; set; }

    [JsonProperty("starts_at")]
    public Optional<DateTimeOffset> StartsAt { get; set; }

    [JsonProperty("ends_at")]
    public Optional<DateTimeOffset> EndsAt { get; set; }
}
