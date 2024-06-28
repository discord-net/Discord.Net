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
            )
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
        MessageChannelActor = new RestMessageChannelActor(client, null, channel);
    }
}

public partial class RestGroupChannel :
    RestChannel,
    IGroupChannel,
    IConstructable<RestGroupChannel, IGroupDMChannelModel, DiscordRestClient>
{
    public IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients => throw new NotImplementedException();

    internal new IGroupDMChannelModel Model { get; }

    [ProxyInterface(typeof(IGroupChannelActor), typeof(IMessageChannelActor))]
    internal override RestGroupChannelActor ChannelActor { get; }

    internal RestGroupChannel(
        DiscordRestClient client,
        IGroupDMChannelModel model,
        RestGroupChannelActor? actor = null
    ) : base(client, model)
    {
        Model = model;
        ChannelActor = actor ?? new(client, this);
    }

    public static RestGroupChannel Construct(DiscordRestClient client, IGroupDMChannelModel model)
        => new(client, model);

}
