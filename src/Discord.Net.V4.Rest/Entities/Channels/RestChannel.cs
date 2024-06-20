using Discord.Models;

namespace Discord.Rest.Channels;

[ExtendInterfaceDefaults(typeof(IChannelActor))]
public partial class RestChannelActor(DiscordRestClient client, ulong id) :
    RestActor<ulong, RestChannel>(client, id),
    IChannelActor;

public partial class RestChannel(DiscordRestClient client, IChannelModel model, RestChannelActor? actor = null) :
    RestEntity<ulong>(client, model.Id),
    IChannel
{
    internal IChannelModel Model { get; } = model;

    internal virtual RestChannelActor Actor { get; } = actor ?? new(client, model.Id);

    public ChannelType Type => (ChannelType)Model.Type;
}
