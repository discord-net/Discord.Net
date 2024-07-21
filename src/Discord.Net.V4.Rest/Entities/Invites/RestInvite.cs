using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Extensions;
using Discord.Rest.Guilds;

namespace Discord.Rest.Invites;

[method: TypeFactory(LastParameter = nameof(invite))]
[ExtendInterfaceDefaults]
public sealed partial class RestInviteActor(
    DiscordRestClient client,
    InviteIdentity invite,
    GuildIdentity? guild = null
) :
    RestActor<string, RestInvite, InviteIdentity>(client, invite),
    IInviteActor
{
    [SourceOfTruth]
    internal RestInvite CreateEntity(IInviteModel model)
        => RestInvite.Construct(Client, guild, model);
}

public sealed partial class RestInvite :
    RestEntity<string>,
    IInvite,
    IContextConstructable<RestInvite, IInviteModel, GuildIdentity?, DiscordRestClient>
{
    public InviteType Type => (InviteType)Model.Type;

    [SourceOfTruth]
    public RestGuildActor? Guild { get; private set; }

    [SourceOfTruth]
    public RestChannelActor? Channel { get; private set; }

    [SourceOfTruth]
    public RestUserActor? Inviter { get; private set; }

    public InviteTargetType? TargetType => (InviteTargetType?)Model.TargetType;

    [SourceOfTruth]
    public RestUserActor? TargetUser { get; private set; }

    public int? ApproximatePresenceCount => Model.ApproximatePresenceCount;

    public int? ApproximateMemberCount => Model.ApproximateMemberCount;

    public DateTimeOffset? ExpiresAt => Model.ExpiresAt;

    [SourceOfTruth]
    public RestGuildScheduledEventActor? GuildScheduledEvent { get; private set; }

    [ProxyInterface(typeof(IInviteActor))]
    internal RestInviteActor Actor { get; }

    internal IInviteModel Model { get; private set; }

    public RestInvite(
        DiscordRestClient client,
        IInviteModel model,
        GuildIdentity? guild = null
    ) : base(client, model.Id)
    {
        Actor = new(client, InviteIdentity.Of(this), guild);
        Model = model;

        Guild = model.GuildId.Map(
            static (id, client, guild) => RestGuildActor.Factory(client, guild ?? GuildIdentity.Of(id)),
            client,
            guild
        );

        Channel = model.ChannelId.Map(
            static (id, client) => RestChannelActor.Factory(client, ChannelIdentity.Of(id)),
            client
        );

        Inviter = model.InviterId.Map(
            static (id, client) => RestUserActor.Factory(client, UserIdentity.Of(id)),
            client
        );

        TargetUser = model.TargetUserId.Map(
            static (id, client) => RestUserActor.Factory(client, UserIdentity.Of(id)),
            client
        );
    }

    public static RestInvite Construct(DiscordRestClient client, GuildIdentity? guild, IInviteModel model)
        => new(client, model, guild);

    public ValueTask UpdateAsync(IInviteModel model, CancellationToken token = default)
    {
        Guild = Guild.UpdateFrom(
            model.GuildId,
            RestGuildActor.Factory,
            Client
        );

        Channel = Channel.UpdateFrom(
            model.ChannelId,
            RestChannelActor.Factory,
            Client
        );

        Inviter = Inviter.UpdateFrom(
            model.InviterId,
            RestUserActor.Factory,
            Client
        );

        TargetUser = TargetUser.UpdateFrom(
            model.TargetUserId,
            RestUserActor.Factory,
            Client
        );

        Model = model;

        return ValueTask.CompletedTask;
    }

    public IInviteModel GetModel() => Model;
}
