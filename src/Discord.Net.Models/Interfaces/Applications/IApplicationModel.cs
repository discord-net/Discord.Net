namespace Discord.Models;

[ModelEquality, HasPartialVariant]
public partial interface IApplicationModel : IEntityModel<ulong>
{
    string Name { get; }
    string? Icon { get; }
    string Description { get; }
    ulong? BotId { get; }
    string[]? RPCOrigins { get; }
    bool IsPublicBot { get; }
    bool BotRequiresCodeGrant { get; }
    string? TermsOfServiceUrl { get; }
    string? PrivacyPolicyUrl { get; }
    ulong? OwnerId { get; }
    string VerifyKey { get; }
    ulong? TeamId { get; }
    ulong? GuildId { get; }
    ulong? PrimarySkuId { get; }
    string? Slug { get; }
    string? CoverImage { get; }
    int? Flags { get; }
    int? ApproximateGuildCount { get; }
    int? ApproximateUserInstallCount { get; }
    string[]? RedirectUris { get; }
    string? InteractionsEndpointUrl { get; }
    string? RoleConnectionsVerificationUrl { get; }
    string[]? Tags { get; }
    IApplicationInstallParamsModel? InstallParams { get; }
    IApplicationIntegrationTypesConfigModel? IntegrationTypesConfig { get; }
    string? CustomInstallUrl { get; }
}
