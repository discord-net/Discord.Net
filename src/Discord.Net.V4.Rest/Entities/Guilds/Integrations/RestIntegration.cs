using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest.Guilds.Integrations;

[method: TypeFactory]
public sealed partial class RestIntegrationActor(
    DiscordRestClient client,
    GuildIdentity guild,
    IntegrationIdentity integration
) :
    RestActor<ulong, RestIntegration, IntegrationIdentity>(client, integration),
    IIntegrationActor
{
    [SourceOfTruth]
    public RestGuildActor Guild { get; } = guild.Actor ?? new(client, guild);
}

public sealed partial class RestIntegration :
    RestEntity<ulong>,
    IIntegration,
    IContextConstructable<RestIntegration, IIntegrationModel, GuildIdentity, DiscordRestClient>
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
        GuildIdentity guild,
        IIntegrationModel model,
        RestIntegrationActor? actor = null
    ) : base(client, model.Id)
    {
        Actor = actor ?? new(client, guild, IntegrationIdentity.Of(this));
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

        Account = model is {AccountId: not null, AccountName: not null}
            ? new IntegrationAccount(model.AccountId, model.AccountName)
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

    public static RestIntegration Construct(DiscordRestClient client, GuildIdentity guild, IIntegrationModel model)
        => new(client, guild, model);

    public IIntegrationModel GetModel() => Model;
}
