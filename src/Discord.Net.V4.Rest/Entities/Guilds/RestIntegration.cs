using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public sealed partial class RestIntegrationActor :
    RestActor<RestIntegrationActor, ulong, RestIntegration, IIntegrationModel>,
    IIntegrationActor
{
    [SourceOfTruth] public RestGuildActor Guild { get; }

    internal override IntegrationIdentity Identity { get; }

    [TypeFactory]
    public RestIntegrationActor(
        DiscordRestClient client,
        GuildIdentity guild,
        IntegrationIdentity integration
    ) : base(client, integration)
    {
        Identity = integration | this;
        Guild = guild.Actor ?? new(client, guild);
    }

    [SourceOfTruth]
    internal override RestIntegration CreateEntity(IIntegrationModel model)
        => RestIntegration.Construct(Client, this, model);
}

public sealed partial class RestIntegration :
    RestEntity<ulong>,
    IIntegration,
    IRestConstructable<RestIntegration, RestIntegrationActor, IIntegrationModel>
{
    [SourceOfTruth] public RestRoleActor? Role { get; }

    [SourceOfTruth] public RestUserActor? User { get; }

    public string Name => Model.Name;

    public IntegrationType Type => (IntegrationType)Model.Type;

    public bool IsEnabled => Model.IsEnabled;

    public bool? IsSyncing => Model.IsSyncing;

    public bool? EmoticonsEnabled => Model.EnableEmoticons;

    public IntegrationExpireBehavior? ExpireBehavior => (IntegrationExpireBehavior?)Model.ExpireBehavior;

    public int? ExpiryGracePeriod => Model.ExpireGracePeriod;

    public IntegrationAccount? Account { get; }

    public DateTimeOffset? SyncedAt => Model.SyncedAt;

    public int? SubscriberCount => Model.SubscriberCount;

    public bool? IsRevoked => Model.IsRevoked;

    public IntegrationApplication? Application { get; }

    public IReadOnlyCollection<string> Scopes => Model.Scopes ?? [];

    [ProxyInterface(typeof(IIntegrationActor), typeof(IGuildRelationship))]
    internal RestIntegrationActor Actor { get; }

    internal IIntegrationModel Model { get; }

    internal RestIntegration(
        DiscordRestClient client,
        IIntegrationModel model,
        RestIntegrationActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        Model = model;

        Role = model.RoleId.Map(
            static (id, client, guild) => new RestRoleActor(client, guild, RoleIdentity.Of(id)),
            client,
            guild
        );

        User = model.UserId
            .Map(
                static (id, client, model) => new RestUserActor(
                    client,
                    UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model, id, client)
                ),
                client,
                model
            );

        Account = model.Account is not null
            ? IntegrationAccount.Construct(client, model.Account)
            : null;

        Application = model.Application is not null
            ? IntegrationApplication.Construct(
                client,
                model.Application,
                model.Application.BotId.HasValue
                    ? new RestUserActor(
                        client,
                        UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model.Application,
                            model.Application.BotId.Value, client)
                    )
                    : null
            )
            : null;
    }

    public static RestIntegration Construct(DiscordRestClient client, RestIntegrationActor actor, IIntegrationModel model)
        => new(client, model, actor);

    public IIntegrationModel GetModel() => Model;
}
