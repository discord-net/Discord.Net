using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Application : IApplicationModel, IEntityModelSource
{
    [JsonPropertyName("id")]
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
    public Optional<bool> IsBotPublic { get; set; }

    [JsonPropertyName("bot_require_code_grant")]
    public Optional<bool> BotRequiresCodeGrant { get; set; }

    [JsonPropertyName("bot")]
    public Optional<User> Bot { get; set; }

    [JsonPropertyName("terms_of_service_url")]
    public Optional<string> TermsOfService { get; set; }

    [JsonPropertyName("privacy_policy_url")]
    public Optional<string> PrivacyPolicy { get; set; }

    [JsonPropertyName("owner")]
    public Optional<User> Owner { get; set; }

    [JsonPropertyName("verify_key")]
    public required string VerifyKey { get; set; }

    [JsonPropertyName("team")]
    public Team? Team { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("install_params")]
    public Optional<InstallParams> InstallParams { get; set; }

    [JsonPropertyName("cover_image")]
    public Optional<string> CoverImage { get; set; }

    [JsonPropertyName("tags")]
    public Optional<string[]> Tags { get; set; }

    [JsonPropertyName("approximate_guild_count")]
    public Optional<int> ApproximateGuildCount { get; set; }

    [JsonPropertyName("guild")]
    public Optional<PartialGuild> PartialGuild { get; set; }

    [JsonPropertyName("custom_install_url")]
    public Optional<string> CustomInstallUrl { get; set; }

    [JsonPropertyName("role_connections_verification_url")]
    public Optional<string> RoleConnectionsUrl { get; set; }

    [JsonPropertyName("interactions_endpoint_url")]
    public Optional<string> InteractionsEndpointUrl { get; set; }

    [JsonPropertyName("redirect_uris")]
    public Optional<string[]> RedirectUris { get; set; }

    ulong? IApplicationModel.BotId => Bot.Map(v => v.Id);

    public IEnumerable<IEntityModel> GetEntities()
    {
        if (Bot.IsSpecified)
            yield return Bot.Value;

        if (Owner.IsSpecified)
            yield return Owner.Value;
    }
}
