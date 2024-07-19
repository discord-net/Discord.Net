using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest.Channels;

[method: TypeFactory]
[ExtendInterfaceDefaults(
    typeof(IGroupChannelActor)
)]
public partial class RestGroupChannelActor(
    DiscordRestClient client,
    GroupChannelIdentity channel
) :
    RestChannelActor(client, channel),
    IGroupChannelActor,
    IRestActor<ulong, RestGroupChannel, GroupChannelIdentity>
{
    public override GroupChannelIdentity Identity { get; } = channel;

    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, channel);

    [SourceOfTruth]
    internal RestGroupChannel CreateEntity(IGroupDMChannelModel model)
        => RestGroupChannel.Construct(Client, model);
}

public partial class RestGroupChannel :
    RestChannel,
    IGroupChannel,
    IConstructable<RestGroupChannel, IGroupDMChannelModel, DiscordRestClient>
{
    public IDefinedLoadableEntityEnumerable<ulong, IUser> Recipients => throw new NotImplementedException();

    [ProxyInterface(
        typeof(IGroupChannelActor),
        typeof(IMessageChannelActor),
        typeof(IEntityProvider<IGroupChannel, IGroupDMChannelModel>)
    )]
    internal override RestGroupChannelActor Actor { get; }

    internal override IGroupDMChannelModel Model => _model;

    private IGroupDMChannelModel _model;

    internal RestGroupChannel(
        DiscordRestClient client,
        IGroupDMChannelModel model,
        RestGroupChannelActor? actor = null
    ) : base(client, model)
    {
        _model = model;
        Actor = actor ?? new(client, GroupChannelIdentity.Of(this));
    }

    public static RestGroupChannel Construct(DiscordRestClient client, IGroupDMChannelModel model)
        => new(client, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGroupDMChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGroupDMChannelModel GetModel() => Model;
}
