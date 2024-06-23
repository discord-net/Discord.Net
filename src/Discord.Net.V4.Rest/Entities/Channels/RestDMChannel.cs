using Discord.Models;
using PropertyChanged;
using System.ComponentModel;

namespace Discord.Rest.Channels;

public partial class RestDMChannelActor(DiscordRestClient client, ulong id) :
    RestChannelActor(client, id),
    IDMChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, null, id);
}

public partial class RestDMChannel(DiscordRestClient client, IDMChannelModel model, RestDMChannelActor? actor = null) :
    RestChannel(client, model),
    IDMChannel,
    INotifyPropertyChanged,
    IConstructable<RestDMChannel, IDMChannelModel, DiscordRestClient>
{
    internal new IDMChannelModel Model { get; set; } = model;

    [ProxyInterface(typeof(IChannelActor), typeof(IMessageChannelActor))]
    internal override RestDMChannelActor Actor { get; } = actor ?? new(client, model.Id);

    [AssignOnPropertyChanged(nameof(Model), nameof(Recipient.Loadable.Id), nameof(Model.RecipientId))]
    public RestLoadableUserActor Recipient { get; } = new(client, model.RecipientId);

    ILoadableEntity<ulong, IUser> IDMChannel.Recipient => Recipient;

    public static RestDMChannel Construct(DiscordRestClient client, IDMChannelModel model)
        => new(client, model);
}
