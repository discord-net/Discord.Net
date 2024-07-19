using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Extensions;
using Discord.Rest.Guilds;

namespace Discord.Rest.Invites;

public sealed partial class RestInviteActor(
    DiscordRestClient client,
    GuildIdentity guild,
    InviteIdentity invite
) :
    RestActor<string, RestInvite, InviteIdentity>,
    IInviteActor
{
    public IInvite CreateEntity(IInviteModel model)
    {

    }
}

public sealed partial class RestInvite :
    RestEntity<string>,
    IInvite
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
        GuildIdentity guild,
        IInviteModel model
    ) : base(client, model.Id)
    {
        Actor = new(client, guild, InviteIdentity.Of(this));
        Model = model;


        Guild = model.GuildId.Map(
            static (id, client) => RestGuildActor.Factory(client, GuildIdentity.Of(id)),
            client
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

    public ValueTask UpdateAsync(IInviteModel model, CancellationToken token = default)
    {
        Channel = Channel.UpdateFrom(
            model.ChannelId,

        )
    }

    public IInviteModel GetModel() => throw new NotImplementedException();
}
