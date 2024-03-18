using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;

using Model = Discord.API.Application;

namespace Discord.Rest;

/// <summary>
///     Represents a REST-based entity that contains information about a Discord application created via the developer portal.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class RestApplication : RestEntity<ulong>, IApplication
{
    protected string _iconId;

    /// <inheritdoc />
    public string Name { get; private set; }
    /// <inheritdoc />
    public string Description { get; private set; }
    /// <inheritdoc />
    public IReadOnlyCollection<string> RPCOrigins { get; private set; }
    /// <inheritdoc />
    public ApplicationFlags Flags { get; private set; }
    /// <inheritdoc />
    public bool? IsBotPublic { get; private set; }
    /// <inheritdoc />
    public bool? BotRequiresCodeGrant { get; private set; }
    /// <inheritdoc />
    public ITeam Team { get; private set; }
    /// <inheritdoc />
    public IUser Owner { get; private set; }
    /// <inheritdoc />
    public string TermsOfService { get; private set; }
    /// <inheritdoc />
    public string PrivacyPolicy { get; private set; }

    /// <inheritdoc />
    public string VerifyKey { get; private set; }
    /// <inheritdoc />
    public string CustomInstallUrl { get; private set; }
    /// <inheritdoc />
    public string RoleConnectionsVerificationUrl { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
    /// <inheritdoc />
    public string IconUrl => CDN.GetApplicationIconUrl(Id, _iconId);

    /// <inheritdoc />
    public PartialGuild Guild { get; private set; }

    /// <inheritdoc />
    public int? ApproximateGuildCount { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> RedirectUris { get; private set; }

    /// <inheritdoc />
    public string InteractionsEndpointUrl { get; private set; }

    /// <inheritdoc />
    public ApplicationInstallParams InstallParams { get; private set; }

    /// <inheritdoc />
    public ApplicationDiscoverabilityState DiscoverabilityState { get; private set; }

    /// <inheritdoc />
    public DiscoveryEligibilityFlags DiscoveryEligibilityFlags { get; private set; }

    /// <inheritdoc />
    public ApplicationExplicitContentFilterLevel ExplicitContentFilterLevel { get; private set; }

    /// <inheritdoc />
    public bool IsHook { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> InteractionEventTypes { get; private set; }

    /// <inheritdoc />
    public ApplicationInteractionsVersion InteractionsVersion { get; private set; }

    /// <inheritdoc />
    public bool IsMonetized { get; private set; }

    /// <inheritdoc />
    public ApplicationMonetizationEligibilityFlags MonetizationEligibilityFlags { get; private set; }

    /// <inheritdoc />
    public ApplicationMonetizationState MonetizationState { get; private set; }

    /// <inheritdoc />
    public ApplicationRpcState RpcState { get; private set; }

    /// <inheritdoc />
    public ApplicationStoreState StoreState { get; private set; }

    /// <inheritdoc />
    public ApplicationVerificationState VerificationState { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> Tags { get; private set; }

    /// <inheritdoc />
    public IReadOnlyDictionary<ApplicationIntegrationType, ApplicationInstallParams> IntegrationTypesConfig { get; private set; }

    internal RestApplication(BaseDiscordClient discord, ulong id)
        : base(discord, id)
    {
    }
    internal static RestApplication Create(BaseDiscordClient discord, Model model)
    {
        var entity = new RestApplication(discord, model.Id);
        entity.Update(model);
        return entity;
    }
    internal void Update(Model model)
    {
        Description = model.Description;
        RPCOrigins = model.RPCOrigins.IsSpecified ? model.RPCOrigins.Value.ToImmutableArray() : ImmutableArray<string>.Empty;
        Name = model.Name;
        _iconId = model.Icon;
        IsBotPublic = model.IsBotPublic.IsSpecified ? model.IsBotPublic.Value : null;
        BotRequiresCodeGrant = model.BotRequiresCodeGrant.IsSpecified ? model.BotRequiresCodeGrant.Value : null;
        Tags = model.Tags.GetValueOrDefault(null)?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
        PrivacyPolicy = model.PrivacyPolicy;
        TermsOfService = model.TermsOfService;

        InstallParams = model.InstallParams.IsSpecified
            ? new ApplicationInstallParams(model.InstallParams.Value.Scopes, (GuildPermission)model.InstallParams.Value.Permission)
            : null;

        if (model.Flags.IsSpecified)
            Flags = model.Flags.Value;
        if (model.Owner.IsSpecified)
            Owner = RestUser.Create(Discord, model.Owner.Value);
        if (model.Team != null)
            Team = RestTeam.Create(Discord, model.Team);

        CustomInstallUrl = model.CustomInstallUrl.IsSpecified ? model.CustomInstallUrl.Value : null;
        RoleConnectionsVerificationUrl = model.RoleConnectionsUrl.IsSpecified ? model.RoleConnectionsUrl.Value : null;
        VerifyKey = model.VerifyKey;

        if (model.PartialGuild.IsSpecified)
            Guild = PartialGuildExtensions.Create(model.PartialGuild.Value);

        InteractionsEndpointUrl = model.InteractionsEndpointUrl.IsSpecified ? model.InteractionsEndpointUrl.Value : null;

        if (model.RedirectUris.IsSpecified)
            RedirectUris = model.RedirectUris.Value.ToImmutableArray();

        ApproximateGuildCount = model.ApproximateGuildCount.IsSpecified ? model.ApproximateGuildCount.Value : null;

        DiscoverabilityState = model.DiscoverabilityState.GetValueOrDefault(ApplicationDiscoverabilityState.None);
        DiscoveryEligibilityFlags = model.DiscoveryEligibilityFlags.GetValueOrDefault(DiscoveryEligibilityFlags.None);
        ExplicitContentFilterLevel = model.ExplicitContentFilter.GetValueOrDefault(ApplicationExplicitContentFilterLevel.Disabled);
        IsHook = model.IsHook;

        InteractionEventTypes = model.InteractionsEventTypes.GetValueOrDefault(Array.Empty<string>()).ToImmutableArray();
        InteractionsVersion = model.InteractionsVersion.GetValueOrDefault(ApplicationInteractionsVersion.Version1);

        IsMonetized = model.IsMonetized;
        MonetizationEligibilityFlags = model.MonetizationEligibilityFlags.GetValueOrDefault(ApplicationMonetizationEligibilityFlags.None);
        MonetizationState = model.MonetizationState.GetValueOrDefault(ApplicationMonetizationState.None);

        RpcState = model.RpcState.GetValueOrDefault(ApplicationRpcState.Disabled);
        StoreState = model.StoreState.GetValueOrDefault(ApplicationStoreState.None);
        VerificationState = model.VerificationState.GetValueOrDefault(ApplicationVerificationState.Ineligible);
            
        var dict = new Dictionary<ApplicationIntegrationType, ApplicationInstallParams>();
        if (model.IntegrationTypesConfig.IsSpecified)
        {
            foreach (var p in model.IntegrationTypesConfig.Value)
            {
                dict.Add(p.Key, new ApplicationInstallParams(p.Value.Scopes ?? Array.Empty<string>(), p.Value.Permission));
            }
        }
        IntegrationTypesConfig = dict.ToImmutableDictionary();
    }

    /// <exception cref="InvalidOperationException">Unable to update this object from a different application token.</exception>
    public async Task UpdateAsync()
    {
        var response = await Discord.ApiClient.GetMyApplicationAsync().ConfigureAwait(false);
        if (response.Id != Id)
            throw new InvalidOperationException("Unable to update this object from a different application token.");
        Update(response);
    }

    /// <summary>
    ///     Gets the name of the application.
    /// </summary>
    /// <returns>
    ///     The name of the application.
    /// </returns>
    public override string ToString() => Name;
    private string DebuggerDisplay => $"{Name} ({Id})";
}
