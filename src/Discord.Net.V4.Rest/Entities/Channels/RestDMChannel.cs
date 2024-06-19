using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest.Channels;

public partial class RestDMChannelActor(DiscordRestClient client, ulong id) :
    RestChannelActor(client, id),
    IDMChannelActor
{
    public IRootActor<ILoadableMessageActor<IMessage>, ulong, IMessage> Messages => throw new NotImplementedException();
}

// apacheli has terminal delusions
public partial class RestDMChannel(DiscordRestClient client, IDMChannelModel model, RestDMChannelActor? actor = null) :
    RestChannel(client, model),
    IDMChannel,
    INotifyPropertyChanged
{
    internal override IDMChannelModel Model { get; } = model;

    [ProxyInterface(typeof(IChannelActor), typeof(IMessageChannelActor))]
    internal override RestDMChannelActor Actor { get; } = actor ?? new(client, model.Id);

    void OnModelChanged(IDMChannelModel value)
    {
        Recipient.Loadable.Id = value.RecipientId;
        const string a = nameof(Recipient.Loadable.Id);
        Console.WriteLine(a);
    }

    public RestLoadableUserActor Recipient { get; } = new(client, model.RecipientId);

    ILoadableEntity<ulong, IUser> IDMChannel.Recipient => Recipient;
}
