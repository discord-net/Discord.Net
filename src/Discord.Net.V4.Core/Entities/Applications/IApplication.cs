using Discord.Models;

namespace Discord;

public interface IApplication :
    ISnowflakeEntity<IApplicationModel>
{
    string Name { get; }   
    string? IconId { get; }
    string Description { get; }
    IReadOnlyCollection<string> RPCOrigins { get; }
    bool IsPublicBot { get; }
    bool BotRequiresCodeGrant { get; }
    IUserActor? Bot { get; }
    string? TermsOfServiceUrl { get; }
    string? PrivacyPolicyUrl { get; }
    IUserActor? Owner { get; }
    string VerifyKey { get; }
    // TODO: Team
    IGuildActor? Guild { get; }
    ulong? PrimarySkuId { get; }
    string? Slug { get; }
    string? CoverImageId { get; }
    ApplicationFlags Flags { get; }
    int? ApproximateGuildCount { get; }
    int? ApproximateUserInstallCount { get; }
    IReadOnlyCollection<string> RedirectUris { get; }
    string? InteractionEndpointUrl { get; }
    string? RoleConnectionsVerificationUrl { get; }
    IReadOnlyCollection<string> Tags { get; }
    ApplicationInstallParams InstallParams { get; }
    IReadOnlyDictionary<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration> IntegrationTypesConfig
    {
        get;
    }
    string? CustomInstallUrl { get; }
}