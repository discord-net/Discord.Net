using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

public partial class RestLoadableGroupChannelActor(DiscordRestClient client, ulong id) :
    RestGroupChannelActor(client, id),
    ILoadableGroupChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IGroupChannel>))]
    internal RestLoadable<ulong, RestGroupChannel, IGroupChannel, Channel> Loadable { get; } =
        new(
            client,
            id,
            Routes.GetChannel(id),
            EntityUtils.FactoryOfDescendantModel<ulong, Channel, RestGroupChannel, GroupDMChannel>(
                (_, model) => RestGroupChannel.Construct(client, model)
            )
        );
}

[ExtendInterfaceDefaults(
    typeof(IGroupChannelActor),
    typeof(IModifiable<ulong, IGroupChannelActor, ModifyGroupDMProperties, ModifyGroupDmParams>)
)]
public partial class RestGroupChannelActor(DiscordRestClient client, ulong id) :
    RestChannelActor(client, id),
    IGroupChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, null, id);
}

public partial class RestGroupChannel(DiscordRestClient client, IGroupDMChannelModel model, RestGroupChannelActor? actor = null) :
    RestChannel(client, model),
    IGroupChannel,
    IConstructable<RestGroupChannel, IGroupDMChannelModel, DiscordRestClient>
{
    internal new IGroupDMChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IGroupChannelActor), typeof(IMessageChannelActor))]
    internal override RestGroupChannelActor Actor { get; } = actor ?? new(client, model.Id);

    public static RestGroupChannel Construct(DiscordRestClient client, IGroupDMChannelModel model)
        => new(client, model);

    public IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients => throw new NotImplementedException();
}
