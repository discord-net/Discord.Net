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
    [SourceOfTruth] public ChannelInviteLinkType.Enumerable.Indexable Invites { get; }

    [SourceOfTruth] public RestUserActor.Indexable.BackLink<RestGroupChannelActor> Recipients { get; }

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

        Recipients = new(this, client, client.Users);

        Invites = new RestChannelInviteActor.Enumerable.Indexable(
            client,
            RestActorProvider.GetOrCreate(
                client,
                Template.Of<ChannelInviteIdentity>(),
                (ChannelIdentity)Identity
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
    [SourceOfTruth] public RestUserActor.Defined.Indexable.BackLink<RestGroupChannelActor> Recipients { get; }

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

        Recipients = new(
            actor,
            actor.Recipients,
            new(client, client.Users, model.Recipients.ToList().AsReadOnly())
        );

        if (model is IModelSourceOfMultiple<IUserModel> users)
        {
            foreach (var user in users.GetModels())
                client.Users[user.Id].AddModelSource(user);
        }
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