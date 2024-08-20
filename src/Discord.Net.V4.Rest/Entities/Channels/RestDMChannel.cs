using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestDMChannelActor :
    RestChannelActor,
    IDMChannelActor,
    IRestActor<ulong, RestDMChannel, DMChannelIdentity, IDMChannelModel>
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
        => RestDMChannel.Construct(Client, this, model);
}

public partial class RestDMChannel :
    RestChannel,
    IDMChannel,
    IRestConstructable<RestDMChannel, RestDMChannelActor, IDMChannelModel>
{
    [ProxyInterface(typeof(IDMChannelActor))]
    internal override RestDMChannelActor Actor { get; }

    internal override IDMChannelModel Model => _model;

    private IDMChannelModel _model;

    internal RestDMChannel(
        DiscordRestClient client,
        IDMChannelModel model,
        RestDMChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;
    }

    public static RestDMChannel Construct(DiscordRestClient client, RestDMChannelActor actor, IDMChannelModel model)
        => new(client, model, actor);

    [CovariantOverride]
    public ValueTask UpdateAsync(IDMChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IDMChannelModel GetModel() => Model;
}
