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
    public RestLoadableUserActor Recipient { get; }

    [ProxyInterface(typeof(IChannelActor), typeof(IMessageChannelActor))]
    internal override RestDMChannelActor Actor { get; }

    internal override IDMChannelModel Model => _model;

    private IDMChannelModel _model;

    internal RestDMChannel(DiscordRestClient client, IDMChannelModel model, RestDMChannelActor? actor = null)
        : base(client, model, actor)
    {
        _model = model;

        Actor = actor ?? new(client, this);
        Recipient =  new(client, UserIdentity.Of(model.RecipientId));
    }

    public static RestDMChannel Construct(DiscordRestClient client, IDMChannelModel model)
        => new(client, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IDMChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    ILoadableEntity<ulong, IUser> IDMChannel.Recipient => Recipient;

    public override IDMChannelModel GetModel() => Model;
}
