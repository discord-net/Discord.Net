using Discord.Gateway.State;
using Discord.Models;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

[ExtendInterfaceDefaults]
public sealed partial class GatewayIntegrationActor :
    GatewayCachedActor<ulong, GatewayIntegration, IntegrationIdentity, IIntegrationModel>,
    IIntegrationActor
{
    [SourceOfTruth, StoreRoot] public GatewayGuildActor Guild { get; }

    internal override IntegrationIdentity Identity { get; }

    [TypeFactory]
    public GatewayIntegrationActor(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IntegrationIdentity integration
    ) : base(client, integration)
    {
        Identity = integration | this;
        Guild = client.Guilds >> guild;
    }
}

public sealed partial class GatewayIntegration :
    GatewayCacheableEntity<GatewayIntegration, ulong, IIntegrationModel>,
    IIntegration
{
    public string Name => Model.Name;

    public IntegrationType Type => Model.Type;

    public bool IsEnabled => Model.IsEnabled;

    public bool? IsSyncing => Model.IsSyncing;

    [SourceOfTruth] public GatewayRoleActor? Role { get; private set; }

    public bool? EmoticonsEnabled => Model.EnableEmoticons;

    public IntegrationExpireBehavior? ExpireBehavior => (IntegrationExpireBehavior?)Model.ExpireBehavior;

    public int? ExpiryGracePeriod => Model.ExpireGracePeriod;

    [SourceOfTruth] public GatewayUserActor? User { get; private set; }

    public IntegrationAccount? Account { get; private set; }

    public DateTimeOffset? SyncedAt => Model.SyncedAt;

    public int? SubscriberCount => Model.SubscriberCount;

    public bool? IsRevoked => Model.IsRevoked;

    public IntegrationApplication? Application { get; private set; }

    public IReadOnlyCollection<string> Scopes { get; private set; }

    [ProxyInterface] internal GatewayIntegrationActor Actor { get; }
    internal IIntegrationModel Model { get; private set; }

    internal GatewayIntegration(
        DiscordGatewayClient client,
        GuildIdentity guild,
        IIntegrationModel model,
        GatewayIntegrationActor? actor = null
    ) : base(client, model.Id)
    {
        Model = model;
        Actor = actor ?? new(client, guild, IntegrationIdentity.Of(this));

        UpdateComputeds(null, model);
    }

    public static GatewayIntegration Construct(
        DiscordGatewayClient client,
        IGatewayConstructionContext context,
        IIntegrationModel model)
    {
        return new GatewayIntegration(
            client,
            context.Path.RequireIdentity(Template.Of<GuildIdentity>()),
            model,
            context.TryGetActor<GatewayIntegrationActor>()
        );
    }

    [MemberNotNull(nameof(Scopes))]
    private void UpdateComputeds(
        IIntegrationModel? oldModel,
        IIntegrationModel model)
    {
        Role = Role.UpdateFrom(
            model.RoleId,
            GatewayRoleActor.Factory,
            Client,
            Guild.Identity
        );

        User = User.UpdateFrom(
            model.UserId,
            GatewayUserActor.Factory,
            Client
        );

        if (oldModel?.Account?.Equals(model.Account) ?? model.Account is not null)
            Account = model.Account is not null
                ? IntegrationAccount.Construct(Client, model.Account)
                : null;

        if (oldModel?.Application?.Equals(model.Application) ?? model.Application is not null)
            Application = model.Application is not null
                ? IntegrationApplication.Construct(Client, model.Application)
                : null;

        if (oldModel?.Scopes?.SequenceEqual(model.Scopes ?? []) ?? model.Scopes is not null)
            Scopes = model.Scopes?.AsReadOnly() ?? (IReadOnlyCollection<string>)Array.Empty<string>();

        Scopes ??= [];
    }

    public override ValueTask UpdateAsync(
        IIntegrationModel model,
        bool updateCache = true,
        CancellationToken token = default)
    {
        if (updateCache) return UpdateCacheAsync(this, model, token);

        UpdateComputeds(Model, model);

        Model = model;

        return ValueTask.CompletedTask;
    }

    public override IIntegrationModel GetModel() => Model;
}
