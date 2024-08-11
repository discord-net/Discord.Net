using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestDMChannelActor :
    RestChannelActor,
    IDMChannelActor,
    IRestActor<ulong, RestDMChannel, DMChannelIdentity>
{
    [SourceOfTruth] public RestUserActor Recipient { get; }

    [ProxyInterface(typeof(IMessageChannelTrait))]
    internal RestMessageChannelTrait<RestDMChannelActor, DMChannelIdentity> MessageChannelTrait { get; }

    [SourceOfTruth] internal sealed override DMChannelIdentity Identity { get; }

    [method: TypeFactory]
    public RestDMChannelActor(
        DiscordRestClient client,
        DMChannelIdentity channel,
        UserIdentity recipient
    ) : base(client, channel)
    {
        Identity = channel | this;

        Recipient = recipient.Actor ?? new(client, recipient);
        MessageChannelTrait = new(client, this, channel);
    }

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
