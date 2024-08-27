using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

using MessageChannelTrait = RestMessageChannelTrait<RestGroupChannelActor, GroupChannelIdentity>;

[ExtendInterfaceDefaults]
public partial class RestGroupChannelActor :
    RestChannelActor,
    IGroupChannelActor,
    IRestActor<ulong, RestGroupChannel, GroupChannelIdentity, IGroupDMChannelModel>
{
    [SourceOfTruth] public ChannelInviteLink.Enumerable.Indexable Invites { get; }

    [SourceOfTruth] public UserLink.Indexable.BackLink<RestGroupChannelActor> Recipients { get; }

    [ProxyInterface(typeof(IMessageChannelTrait))]
    internal MessageChannelTrait MessageChannelTrait { get; }

    [SourceOfTruth] internal sealed override GroupChannelIdentity Identity { get; }

    [TypeFactory]
    public RestGroupChannelActor(
        DiscordRestClient client,
        GroupChannelIdentity channel
    ) : base(client, channel)
    {
        Identity = channel | this;

        MessageChannelTrait = new(client, this, channel);

        Recipients =
            (UserLink.Indexable.BackLink<RestGroupChannelActor>?) channel.Entity?.Recipients
            ?? new RestUserLink.Indexable.BackLink<RestGroupChannelActor>(
                this,
                client,
                client.Users
            );

        Invites = new RestChannelInviteLink.Enumerable.Indexable(
            client,
            new RestActorProvider<string, RestChannelInviteActor>(
                (client, id) => new RestChannelInviteActor(client, Identity, ChannelInviteIdentity.Of(id))
            ),
            Routes.GetChannelInvites(Id).AsRequiredProvider()
        );
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal RestGroupChannel CreateEntity(IGroupDMChannelModel model)
        => RestGroupChannel.Construct(Client, this, model);
}

public partial class RestGroupChannel :
    RestChannel,
    IGroupChannel,
    IRestConstructable<RestGroupChannel, RestGroupChannelActor, IGroupDMChannelModel>
{
    [SourceOfTruth] public UserLink.Defined.Indexable.BackLink<RestGroupChannelActor> Recipients { get; }

    [ProxyInterface(
        typeof(IGroupChannelActor),
        typeof(IMessageChannelTrait),
        typeof(IEntityProvider<IGroupChannel, IGroupDMChannelModel>)
    )]
    internal override RestGroupChannelActor Actor { get; }

    internal override IGroupDMChannelModel Model => _model;

    private IGroupDMChannelModel _model;

    internal RestGroupChannel(
        DiscordRestClient client,
        IGroupDMChannelModel model,
        RestGroupChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;

        Recipients = new RestUserLink.Defined.Indexable.BackLink<RestGroupChannelActor>(
            actor,
            Client,
            Client.Users,
            model.Recipients.ToList().AsReadOnly()
        );
    }

    public static RestGroupChannel Construct(
        DiscordRestClient client,
        RestGroupChannelActor actor,
        IGroupDMChannelModel model)
        => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGroupDMChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGroupDMChannelModel GetModel() => Model;
}