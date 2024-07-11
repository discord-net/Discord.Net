using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

public partial class RestLoadableGroupChannelActor :
    RestGroupChannelActor,
    ILoadableGroupChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGroupChannel>))]
    internal RestLoadable<ulong, RestGroupChannel, IGroupChannel, IChannelModel> Loadable { get; }

    internal RestLoadableGroupChannelActor(
        DiscordRestClient client,
        IdentifiableEntityOrModel<ulong, RestGroupChannel, IGroupDMChannelModel> channel
    ) : base(client, channel)
    {
        Loadable = new RestLoadable<ulong, RestGroupChannel, IGroupChannel, IChannelModel>(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestGroupChannel, GroupDMChannel>(
                (_, model) => RestGroupChannel.Construct(client, model)
            ).Invoke
        );
    }
}

[ExtendInterfaceDefaults(
    typeof(IGroupChannelActor),
    typeof(IModifiable<ulong, IGroupChannelActor, ModifyGroupDMProperties, ModifyGroupDmParams>)
)]
public partial class RestGroupChannelActor :
    RestChannelActor,
    IGroupChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; }

    internal RestGroupChannelActor(
        DiscordRestClient client,
        IdentifiableEntityOrModel<ulong, RestGroupChannel, IGroupDMChannelModel> channel
    ) : base(client, channel)
    {
        MessageChannelActor = new RestMessageChannelActor(client, channel);
    }

    public IGroupChannel CreateEntity(IGroupDMChannelModel model)
        => RestGroupChannel.Construct(Client, model);
}

public partial class RestGroupChannel :
    RestChannel,
    IGroupChannel,
    IConstructable<RestGroupChannel, IGroupDMChannelModel, DiscordRestClient>
{
    public IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients => throw new NotImplementedException();

    internal override IGroupDMChannelModel Model => _model;

    [ProxyInterface(
        typeof(IGroupChannelActor),
        typeof(IMessageChannelActor),
        typeof(IEntityProvider<IGroupChannel, IGroupDMChannelModel>)
    )]
    internal override RestGroupChannelActor Actor { get; }

    private IGroupDMChannelModel _model;

    internal RestGroupChannel(
        DiscordRestClient client,
        IGroupDMChannelModel model,
        RestGroupChannelActor? actor = null
    ) : base(client, model)
    {
        _model = model;
        Actor = actor ?? new(client, this);
    }

    public static RestGroupChannel Construct(DiscordRestClient client, IGroupDMChannelModel model)
        => new(client, model);

    public ValueTask UpdateAsync(IGroupDMChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGroupDMChannelModel GetModel() => Model;
}
