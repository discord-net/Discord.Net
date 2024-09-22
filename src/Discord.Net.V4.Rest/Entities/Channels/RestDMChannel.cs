using Discord.Models;
using System.ComponentModel;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestDMChannelActor :
    RestChannelActor,
    IRestActor<RestDMChannelActor, ulong, RestDMChannel, IDMChannelModel>,
    IDMChannelActor,
    IRestMessageChannelTrait
{
    [SourceOfTruth] internal sealed override DMChannelIdentity Identity { get; }

    [method: TypeFactory]
    public RestDMChannelActor(
        DiscordRestClient client,
        DMChannelIdentity channel
    ) : base(client, channel)
    {
        Identity = channel | this;
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
    [SourceOfTruth]
    public RestUserActor Recipient { get; }
    
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

        Recipient = client.Users[model.RecipientId];

        if (model is IModelSourceOf<IUserModel> userModelSource)
            Recipient.AddModelSource(userModelSource.Model);
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
