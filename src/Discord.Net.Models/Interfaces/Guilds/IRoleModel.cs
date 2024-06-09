namespace Discord.Models;

public interface IRoleModel : IEntityModel<ulong>
{
    string Name { get; }
    uint Color { get; }
    bool IsHoisted { get; }
    string? Icon { get; }
    string? UnicodeEmoji { get; }
    int Position { get; }
    ulong Permissions { get; }
    bool IsManaged { get; }
    bool IsMentionable { get; }
    int Flags { get; }

    // tags
    ulong? BotId { get; }
    ulong? IntegrationId { get; }
    bool IsPremiumSubscriberRole { get; }
    ulong? SubscriptionListingId { get; }
    bool AvailableForPurchase { get; }
    bool IsGuildConnection { get; }
}
