using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestTextChannelActor :
    RestThreadableChannelActor,
    ITextChannelActor,
    IRestActor<RestTextChannelActor, ulong, RestTextChannel, IGuildTextChannelModel>,
    IRestMessageChannelTrait,
    IRestIntegrationChannelTrait.WithIncoming.WithChannelFollower
{
    [SourceOfTruth] internal override TextChannelIdentity Identity { get; }

    [method: TypeFactory]
    public RestTextChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        TextChannelIdentity channel
    ) : base(client, guild, channel)
    {
        Identity = channel | this;
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestTextChannel CreateEntity(IGuildTextChannelModel model)
        => RestTextChannel.Construct(Client, this, model);
}

public partial class RestTextChannel :
    RestThreadableChannel,
    ITextChannel,
    IRestConstructable<RestTextChannel, RestTextChannelActor, IGuildTextChannelModel>
{
    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public int SlowModeInterval => Model.RatelimitPerUser;

    [ProxyInterface(
        typeof(ITextChannelActor),
        typeof(IMessageChannelTrait),
        typeof(IEntityProvider<ITextChannel, IGuildTextChannelModel>)
    )]
    internal override RestTextChannelActor Actor { get; }

    internal override IGuildTextChannelModel Model => _model;

    private IGuildTextChannelModel _model;

    internal RestTextChannel(
        DiscordRestClient client,
        IGuildTextChannelModel model,
        RestTextChannelActor actor
    ) : base(client, model, actor)
    {
        _model = model;
        Actor = actor;
    }

    public static RestTextChannel Construct(
        DiscordRestClient client,
        RestTextChannelActor actor,
        IGuildTextChannelModel model
    )
    {
        switch (model)
        {
            case IGuildNewsChannelModel newsModel:
                return RestNewsChannel.Construct(
                    client,
                    actor as RestNewsChannelActor ?? actor.Guild.Channels.News[model.Id],
                    newsModel
                );
            default:
                return new(client, model, actor);
        }
    }

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IGuildTextChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IGuildTextChannelModel GetModel() => Model;
}