using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[HasPartialVariant]
public sealed class Application : 
    IApplicationModel, 
    IModelSourceOfMultiple<IPartialUserModel>, 
    IModelSourceOf<IPartialGuildModel?>
{
    [JsonPropertyName("id"), JsonRequired]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }

    [JsonPropertyName("rpc_origins")]
    public Optional<string[]> RPCOrigins { get; set; }

    [JsonPropertyName("bot_public")]
    public bool IsPublicBot { get; set; }

    [JsonPropertyName("bot_require_code_grant")]
    public bool BotRequiresCodeGrant { get; set; }

    [JsonPropertyName("bot")]
    public Optional<PartialUser> Bot { get; set; }

    [JsonPropertyName("terms_of_service_url")]
    public Optional<string> TermsOfService { get; set; }

    [JsonPropertyName("privacy_policy_url")]
    public Optional<string> PrivacyPolicy { get; set; }

    [JsonPropertyName("owner")]
    public Optional<PartialUser> Owner { get; set; }

    [JsonPropertyName("verify_key")]
    public required string VerifyKey { get; set; }

    [JsonPropertyName("team")]
    public Team? Team { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }
    
    [JsonPropertyName("guild")]
    public Optional<PartialGuild> Guild { get; set; }
    
    [JsonPropertyName("primary_sku_id")]
    public Optional<ulong> PrimarySkuId { get; set; }
    
    [JsonPropertyName("slug")]
    public Optional<string> Slug { get; set; }
    
    [JsonPropertyName("cover_image")]
    public Optional<string> CoverImage { get; set; }
    
    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("approximate_guild_count")]
    public Optional<int> ApproximateGuildCount { get; set; }
    
    [JsonPropertyName("approximate_user_install_count")]
    public Optional<int> ApproximateUserInstallCount { get; set; }
    
    [JsonPropertyName("redirect_uris")]
    public Optional<string[]> RedirectUris { get; set; }
    
    [JsonPropertyName("interactions_endpoint_url")]
    public Optional<string> InteractionsEndpointUrl { get; set; }
    
    [JsonPropertyName("role_connections_verification_url")]
    public Optional<string> RoleConnectionsUrl { get; set; }
    
    [JsonPropertyName("tags")]
    public Optional<string[]> Tags { get; set; }
    
    [JsonPropertyName("install_params")]
    public Optional<InstallParams> InstallParams { get; set; }
    
    [JsonPropertyName("integration_types_config")]
    public Optional<ApplicationIntegrationTypesConfig> IntegrationTypesConfig { get; set; }
    
    [JsonPropertyName("custom_install_url")]
    public Optional<string> CustomInstallUrl { get; set; }

    ulong? IApplicationModel.BotId => Bot.Map(v => v.Id).ToNullable();
    string[]? IApplicationModel.RPCOrigins => ~RPCOrigins;
    string? IApplicationModel.TermsOfServiceUrl => ~TermsOfService;
    string? IApplicationModel.PrivacyPolicyUrl => ~PrivacyPolicy;
    ulong? IApplicationModel.OwnerId => Owner.Map(v => v.Id).ToNullable();
    ulong? IApplicationModel.TeamId => Team?.Id;
    ulong? IApplicationModel.GuildId => GuildId.ToNullable();
    ulong? IApplicationModel.PrimarySkuId => PrimarySkuId.ToNullable();
    string? IApplicationModel.Slug => ~Slug;
    string? IApplicationModel.CoverImage => ~CoverImage;
    int? IApplicationModel.Flags => Flags.ToNullable();
    int? IApplicationModel.ApproximateGuildCount => ApproximateGuildCount.ToNullable();
    int? IApplicationModel.ApproximateUserInstallCount => ApproximateUserInstallCount.ToNullable();
    string[]? IApplicationModel.RedirectUris => ~RedirectUris;
    string? IApplicationModel.InteractionsEndpointUrl => ~InteractionsEndpointUrl;
    string? IApplicationModel.RoleConnectionsVerificationUrl => ~RoleConnectionsUrl;
    string[]? IApplicationModel.Tags => ~Tags;
    IApplicationInstallParamsModel? IApplicationModel.InstallParams => ~InstallParams;
    IApplicationIntegrationTypesConfigModel? IApplicationModel.IntegrationTypesConfig => ~IntegrationTypesConfig;
    string? IApplicationModel.CustomInstallUrl => ~CustomInstallUrl;
    
    IEnumerable<IPartialUserModel> IModelSourceOfMultiple<IPartialUserModel>.GetModels()
    {
        if (Bot.IsSpecified)
            yield return Bot.Value;

        if (Owner.IsSpecified)
            yield return Owner.Value;
    }

    IPartialGuildModel? IModelSourceOf<IPartialGuildModel?>.Model => ~Guild;
}
