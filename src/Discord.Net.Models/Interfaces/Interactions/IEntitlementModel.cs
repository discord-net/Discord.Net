namespace Discord.Models;

[ModelEquality]
public partial interface IEntitlementModel : IEntityModel<ulong>
{
    ulong SkuId { get; }
    ulong ApplicationId { get; }
    ulong? UserId { get; }
    int Type { get; }
    bool IsDeleted { get; }
    DateTimeOffset? StartsAt { get; }
    DateTimeOffset? EndsAt { get; }
    ulong? GuildId { get; }
    bool? Consumed { get; }
}
