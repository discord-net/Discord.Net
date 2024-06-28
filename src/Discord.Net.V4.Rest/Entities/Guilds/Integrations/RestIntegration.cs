using Discord;
using Discord.Models;
using PropertyChanged;
using System.ComponentModel;

namespace Discord.Rest.Guilds.Integrations;

public sealed partial class RestIntegrationActor(DiscordRestClient client, ulong guildId, ulong id, RestGuild? guild = null) :
    RestActor<ulong, RestIntegration>(client, id),
    IIntegrationActor
{
    public RestLoadableGuildActor Guild { get; } = new(client, guildId, guild);

    ILoadableGuildActor IGuildRelationship.Guild => Guild;
}

public sealed partial class RestIntegration(
    DiscordRestClient client,
    ulong guildId,
    IIntegrationModel model,
    RestIntegrationActor? actor = null,
    RestGuild? guild = null
):
    RestEntity<ulong>(client, model.Id),
    IIntegration,
    IContextConstructable<RestIntegration, IIntegrationModel, RestIntegration.Context, DiscordRestClient>,
    INotifyPropertyChanged
{
    public readonly record struct Context(
        ulong GuildId,
        RestGuild? Guild = null
    );

    [ProxyInterface(typeof(IIntegrationActor), typeof(IGuildRelationship))]
    internal RestIntegrationActor Actor { get; } = actor ?? new(client, guildId, model.Id, guild);

    [OnChangedMethod(nameof(OnModelUpdated))]
    internal IIntegrationModel Model { get; set; } = model;

    public static RestIntegration Construct(DiscordRestClient client, IIntegrationModel model, Context context)
        => new(client, context.GuildId, model, guild: context.Guild);

    private void OnModelUpdated()
    {
        if (Model.AccountId != Account?.Id || Model.AccountName != Account?.Name)
        {
            Account = Model is {AccountId: not null, AccountName: not null}
                ? new IntegrationAccount(Model.AccountId, Model.AccountName)
                : null;
        }

        if (IsApplicationOutOfDate)
        {
            Application = Model.Application is not null
                ? IntegrationApplication.Construct(
                    Client,
                    Model.Application,
                    Model.Application.BotId.HasValue
                        ? new RestLoadableUserActor(
                            Client,
                            Model.Application.BotId.Value,
                            Model.GetReferencedEntityModel<ulong, IUserModel>(Model.Application.BotId.Value)
                        ) : null
                ) : null;
        }

        if (IsRoleOutOfDate)
        {
            Role = Model.RoleId.HasValue
                ? new RestRoleActor(Client, guildId, Model.RoleId.Value)
                : null;
        }

        if (IsUserOutOfDate)
        {
            User = Model.UserId.HasValue
                ? new RestLoadableUserActor(
                    Client,
                    Model.UserId.Value,
                    Model.GetReferencedEntityModel<ulong, IUserModel>(Model.UserId.Value))
                : null;
        }
    }

    #region Actors

    [VersionOn(nameof(Model.RoleId), nameof(model.RoleId))]
    public IRoleActor? Role { get; private set; } =
        model.RoleId.HasValue
            ? new RestRoleActor(client, guildId, model.RoleId.Value)
            : null;

    [VersionOn(nameof(Model.RoleId), nameof(model.RoleId))]
    public ILoadableUserActor? User { get; private set; } =
        model.UserId.HasValue
            ? new RestLoadableUserActor(
                client,
                model.UserId.Value,
                model.GetReferencedEntityModel<ulong, IUserModel>(model.UserId.Value))
            : null;

    #endregion

    #region Properties

    public string Name => Model.Name;

    public IntegrationType Type => (IntegrationType)Model.Type;

    public bool IsEnabled => Model.IsEnabled;

    public bool? IsSyncing => Model.IsSyncing;

    public bool? EmoticonsEnabled => Model.EnableEmoticons;

    public IntegrationExpireBehavior? ExpireBehavior => (IntegrationExpireBehavior?)Model.ExpireBehavior;

    public int? ExpiryGracePeriod => Model.ExpireGracePeriod;

    public IntegrationAccount? Account { get; private set; } =
        model is {AccountId: not null, AccountName: not null}
            ? new IntegrationAccount(model.AccountId, model.AccountName)
            : null;

    public DateTimeOffset? SyncedAt => Model.SyncedAt;

    public int? SubscriberCount => Model.SubscriberCount;

    public bool? IsRevoked => Model.IsRevoked;

    [VersionOn(nameof(Model.Application), nameof(model.Application))]
    public IntegrationApplication? Application { get; private set; } =
        model.Application is not null
            ? IntegrationApplication.Construct(
                client,
                model.Application,
                model.Application.BotId.HasValue
                    ? new RestLoadableUserActor(
                        client,
                        model.Application.BotId.Value,
                        model.GetReferencedEntityModel<ulong, IUserModel>(model.Application.BotId.Value)
                    ) : null
            ) : null;

    public string[] Scopes => Model.Scopes ?? [];

    #endregion
}
