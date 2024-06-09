using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Role : IRoleModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("color")]
    public uint Color { get; set; }

    [JsonPropertyName("hoist")]
    public bool Hoist { get; set; }

    [JsonPropertyName("icon")]
    public Optional<string?> Icon { get; set; }

    [JsonPropertyName("unicode_emoji")]
    public Optional<string?> UnicodeEmoji { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("permissions")]
    public ulong Permissions { get; set; }

    [JsonPropertyName("managed")]
    public bool Managed { get; set; }

    [JsonPropertyName("mentionable")]
    public bool Mentionable { get; set; }

    [JsonPropertyName("tags")]
    public Optional<RoleTags> Tags { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    bool IRoleModel.IsHoisted => Hoist;

    string? IRoleModel.Icon => Icon;

    string? IRoleModel.UnicodeEmoji => UnicodeEmoji;

    bool IRoleModel.IsManaged => Managed;

    bool IRoleModel.IsMentionable => Mentionable;

    int IRoleModel.Flags => Flags;

    ulong? IRoleModel.BotId => Tags.Map(v => v.BotId);

    ulong? IRoleModel.IntegrationId => Tags.Map(v => v.IntegrationId);

    // see https://discord.com/developers/docs/topics/permissions#role-object-role-tags-structure
    bool IRoleModel.IsPremiumSubscriberRole => Tags.Map(v => v.IsPremiumSubscriber).IsSpecified;
    ulong? IRoleModel.SubscriptionListingId => Tags.Map(v => v.SubscriptionListingId);

    bool IRoleModel.AvailableForPurchase => Tags.Map(v => v.IsAvailableForPurchase).IsSpecified;

    bool IRoleModel.IsGuildConnection => Tags.Map(v => v.HasGuildConnections).IsSpecified;
}
