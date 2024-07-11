using System.Numerics;

namespace Discord.Models;

[ModelEquality]
public partial interface IRoleModel : IEntityModel<ulong>
{
    string Name { get; }
    uint Color { get; }
    bool IsHoisted { get; }
    string? Icon { get; }
    string? UnicodeEmoji { get; }
    int Position { get; }
    BigInteger Permissions { get; }
    bool IsManaged { get; }
    bool IsMentionable { get; }
    int Flags { get; }

    IRoleTagsModel? Tags { get; }
}

public interface IRoleTagsModel
{
    ulong? BotId { get; }
    ulong? IntegrationId { get; }
    bool IsPremiumSubscriberRole { get; }
    ulong? SubscriptionListingId { get; }
    bool AvailableForPurchase { get; }
    bool IsGuildConnection { get; }
}
