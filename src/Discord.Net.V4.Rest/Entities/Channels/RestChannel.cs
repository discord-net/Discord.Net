using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestChannelActor :
    RestActor<RestChannelActor, ulong, RestChannel, IChannelModel>,
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
            IDMChannelModel dmChannelModel => RestDMChannel.Construct(
                client,
                actor as RestDMChannelActor ?? client.Channels.DM[model.Id],
                dmChannelModel
            ),
            IGroupDMChannelModel groupChannelModel => RestGroupChannel.Construct(
                client,
                actor as RestGroupChannelActor ?? client.Channels.Group[model.Id],
                groupChannelModel
            ),
            IGuildChannelModel guildChannelModel => RestGuildChannel.Construct(
                client,
                // TODO: client.Guilds[guildChannelBase.GuildId].Channels[model.Id] leads to bad cast later on
                actor as RestGuildChannelActor ?? client.Guilds[guildChannelModel.GuildId].Channels[model.Id], 
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