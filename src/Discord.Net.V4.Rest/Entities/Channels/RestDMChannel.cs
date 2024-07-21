using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest.Channels;

[method: TypeFactory]
public partial class RestDMChannelActor(
    DiscordRestClient client,
    DMChannelIdentity channel,
    UserIdentity recipient
) :
    RestChannelActor(client, channel),
    IDMChannelActor,
    IRestActor<ulong, RestDMChannel, DMChannelIdentity>
{
    public override DMChannelIdentity Identity { get; } = channel;

    [SourceOfTruth] public RestUserActor Recipient { get; } = recipient.Actor ?? new(client, recipient);

    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, channel);

    [SourceOfTruth]
    [CovariantOverride]
    internal RestDMChannel CreateEntity(IDMChannelModel model)
        => RestDMChannel.Construct(Client, model);
}

public partial class RestDMChannel :
    RestChannel,
    IDMChannel,
    IConstructable<RestDMChannel, IDMChannelModel, DiscordRestClient>,
    IContextConstructable<RestDMChannel, IDMChannelModel, UserIdentity, DiscordRestClient>
{
    [ProxyInterface(typeof(IDMChannelActor))]
    internal override RestDMChannelActor Actor { get; }

    internal override IDMChannelModel Model => _model;

    private IDMChannelModel _model;

    internal RestDMChannel(
        DiscordRestClient client,
        IDMChannelModel model,
        UserIdentity? recipient = null,
        RestDMChannelActor? actor = null
    ) : base(client, model, actor)
    {
        _model = model;

        Actor = actor ?? new(client, DMChannelIdentity.Of(this), recipient ?? UserIdentity.Of(model.RecipientId));
    }

    public static RestDMChannel Construct(DiscordRestClient client, UserIdentity recipient, IDMChannelModel model)
        => new(client, model, recipient);

    public static RestDMChannel Construct(DiscordRestClient client, IDMChannelModel model)
        => new(client, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IDMChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IDMChannelModel GetModel() => Model;
}
