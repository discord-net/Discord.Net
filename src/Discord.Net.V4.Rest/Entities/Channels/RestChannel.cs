using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestChannelActor :
    RestActor<ulong, RestChannel, ChannelIdentity, IChannelModel>,
    IChannelActor
{
    internal override ChannelIdentity Identity { get; }

    [method: TypeFactory]
    public RestChannelActor(
        DiscordRestClient client,
        ChannelIdentity channel
    ) : base(client, channel)
    {
        Identity = channel | this;
    }

    [SourceOfTruth]
    internal override RestChannel CreateEntity(IChannelModel model)
        => RestChannel.Construct(Client, this, model);
}

public partial class RestChannel :
    RestEntity<ulong>,
    IChannel,
    IRestConstructable<RestChannel, RestChannelActor, IChannelModel>
{
    public ChannelType Type => (ChannelType) Model.Type;

    [ProxyInterface(
        typeof(IChannelActor),
        typeof(IEntityProvider<IChannel, IChannelModel>)
    )]
    internal virtual RestChannelActor Actor { get; }

    internal virtual IChannelModel Model => _model;

    private IChannelModel _model;

    internal RestChannel(
        DiscordRestClient client,
        IChannelModel model,
        RestChannelActor actor
    ) : base(client, model.Id)
    {
        _model = model;
        Actor = actor;
    }

    public static RestChannel Construct(DiscordRestClient client, RestChannelActor actor, IChannelModel model)
    {
        return model switch
        {
            IDMChannelModel dmChannelModel => RestDMChannel.Construct(client, dmChannelModel),
            IGroupDMChannelModel groupDMChannel => RestGroupChannel.Construct(client, groupDMChannel),
            IGuildChannelModel guildChannelBase => RestGuildChannel.Construct(
                client,
                GuildIdentity.Of(guildChannelBase.GuildId),
                model
            ),
            _ => new RestChannel(client, model, actor)
        };
    }

    public virtual ValueTask UpdateAsync(IChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return ValueTask.CompletedTask;
    }

    public virtual IChannelModel GetModel() => Model;
}