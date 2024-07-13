using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest.Guilds.Integrations;

public sealed partial class RestIntegrationActor(
    DiscordRestClient client,
    GuildIdentity guild,
    IntegrationIdentity integration
):
    RestActor<ulong, RestIntegration, IntegrationIdentity>(client, integration),
    IIntegrationActor
{
    [SourceOfTruth]
    public RestLoadableGuildActor Guild { get; } = new(client, guild);
}

public sealed partial class RestIntegration :
    RestEntity<ulong>,
    IIntegration,
    IContextConstructable<RestIntegration, IIntegrationModel, GuildIdentity, DiscordRestClient>
{
    [ProxyInterface(typeof(IIntegrationActor), typeof(IGuildRelationship))]
    internal RestIntegrationActor Actor { get; }

    internal IIntegrationModel Model { get; private set; }

    public RestIntegration(DiscordRestClient client,
        GuildIdentity guild,
        IIntegrationModel model,
        RestIntegrationActor? actor = null) : base(client, model.Id)
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
                static (id, client, model) => new RestLoadableUserActor(
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
                    ? new RestLoadableUserActor(
                        client,
                        UserIdentity.FromReferenced<RestUser, DiscordRestClient>(model.Application, model.Application.BotId.Value, client)
                    ) : null
            ) : null;
    }

    public static RestIntegration Construct(DiscordRestClient client, IIntegrationModel model, GuildIdentity guild)
        => new(client, guild, model);

    public IIntegrationModel GetModel() => Model;

    private void OnModelUpdated()
    {
        // if (Model.AccountId != Account?.Id || Model.AccountName != Account?.Name)
        // {
        //     Account = Model is {AccountId: not null, AccountName: not null}
        //         ? new IntegrationAccount(Model.AccountId, Model.AccountName)
        //         : null;
        // }
        //
        // if (IsApplicationOutOfDate)
        // {
        //     Application = Model.Application is not null
        //         ? IntegrationApplication.Construct(
        //             Client,
        //             Model.Application,
        //             Model.Application.BotId.HasValue
        //                 ? new RestLoadableUserActor(
        //                     Client,
        //                     Model.Application.BotId.Value,
        //                     Model.GetReferencedEntityModel<ulong, IUserModel>(Model.Application.BotId.Value)
        //                 ) : null
        //         ) : null;
        // }
        //
        // if (IsRoleOutOfDate)
        // {
        //     Role = Model.RoleId.HasValue
        //         ? new RestRoleActor(Client, guildId, Model.RoleId.Value)
        //         : null;
        // }
        //
        // if (IsUserOutOfDate)
        // {
        //     User = Model.UserId.HasValue
        //         ? new RestLoadableUserActor(
        //             Client,
        //             Model.UserId.Value,
        //             Model.GetReferencedEntityModel<ulong, IUserModel>(Model.UserId.Value))
        //         : null;
        // }
    }

    #region Actors

    public IRoleActor? Role { get; private set; }

    public ILoadableUserActor? User { get; private set; }

    #endregion

    #region Properties

    public string Name => Model.Name;

    public IntegrationType Type => (IntegrationType)Model.Type;

    public bool IsEnabled => Model.IsEnabled;

    public bool? IsSyncing => Model.IsSyncing;

    public bool? EmoticonsEnabled => Model.EnableEmoticons;

    public IntegrationExpireBehavior? ExpireBehavior => (IntegrationExpireBehavior?)Model.ExpireBehavior;

    public int? ExpiryGracePeriod => Model.ExpireGracePeriod;

    public IntegrationAccount? Account { get; private set; }

    public DateTimeOffset? SyncedAt => Model.SyncedAt;

    public int? SubscriberCount => Model.SubscriberCount;

    public bool? IsRevoked => Model.IsRevoked;

    public IntegrationApplication? Application { get; private set; }

    public string[] Scopes => Model.Scopes ?? [];

    #endregion
}
