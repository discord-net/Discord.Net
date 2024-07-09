using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest.Channels;

public partial class RestDMChannelActor(
    DiscordRestClient client,
    IdentifiableEntityOrModel<ulong, RestDMChannel, IDMChannelModel> channel
):
    RestChannelActor(client, channel),
    IDMChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, channel);
}

public partial class RestDMChannel :
    RestChannel,
    IDMChannel,
    IConstructable<RestDMChannel, IDMChannelModel, DiscordRestClient>
{
    [AssignOnPropertyChanged(nameof(Model), nameof(Recipient.Loadable.Id), nameof(Model.RecipientId))]
    public RestLoadableUserActor Recipient { get; }

    internal override IDMChannelModel Model => _model;

    [ProxyInterface(typeof(IChannelActor), typeof(IMessageChannelActor))]
    internal override RestDMChannelActor ChannelActor { get; }

    private IDMChannelModel _model;

    internal RestDMChannel(DiscordRestClient client, IDMChannelModel model, RestDMChannelActor? actor = null)
        : base(client, model, actor)
    {
        ChannelActor = actor ?? new(client, this);
        _model = model;

        Recipient =  new(client, UserIdentity.Of(model.RecipientId));
    }

    public static RestDMChannel Construct(DiscordRestClient client, IDMChannelModel model)
        => new(client, model);

    public ValueTask UpdateAsync(IDMChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IDMChannelModel GetModel() => Model;

    ILoadableEntity<ulong, IUser> IDMChannel.Recipient => Recipient;
}
