namespace Discord.Models;

[ModelEquality]
public partial interface IInteractionModel : IEntityModel<ulong>
{
    ulong ApplicationId { get; }
    int Type { get; }
    IInteractionDataModel? Data { get; }
    ulong? GuildId { get; }
    ulong? ChannelId { get; }
    ulong? UserId { get; }
    string Token { get; }
    int Version { get; }
    ulong? MessageId { get; }
    string? AppPermissions { get; }
    string? Locale { get; }
    string? GuildLocale { get; }
    IEnumerable<IEntitlementModel> Entitlements { get; }
    IApplicationIntegrationTypes? AuthorizingIntegrationOwners { get; }
    int Context { get; }
}
