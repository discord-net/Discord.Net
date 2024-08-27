using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest;

namespace Discord.Rest;

using MessageChannelTrait = RestMessageChannelTrait<RestTextChannelActor, TextChannelIdentity>;
using IncomingIntegrationChannelTrait = RestIncomingIntegrationChannelTrait<RestTextChannelActor, RestTextChannel, TextChannelIdentity>;
using ChannelFollowerIntegrationChannelTrait = RestChannelFollowerIntegrationChannelTrait<RestTextChannelActor, RestTextChannel, TextChannelIdentity>;


[ExtendInterfaceDefaults]
public partial class RestTextChannelActor :
    RestThreadableChannelActor,
    ITextChannelActor,
    IRestActor<ulong, RestTextChannel, TextChannelIdentity, IGuildTextChannelModel>
{
    [ProxyInterface(typeof(IMessageChannelTrait))]
    internal MessageChannelTrait MessageChannelTrait { get; }

    [ProxyInterface(typeof(IIncomingIntegrationChannelTrait))]
    internal IncomingIntegrationChannelTrait IncomingIntegrationChannelTrait { get; }

    [ProxyInterface(typeof(IChannelFollowerIntegrationChannelTrait))]
    internal ChannelFollowerIntegrationChannelTrait ChannelFollowerIntegrationChannelTrait { get; }

    [SourceOfTruth] internal override TextChannelIdentity Identity { get; }

    [method: TypeFactory]
    public RestTextChannelActor(
        DiscordRestClient client,
        GuildIdentity guild,
        TextChannelIdentity channel
    ) : base(client, guild, channel)
    {
        channel = Identity = channel | this;

        MessageChannelTrait = new MessageChannelTrait(client, this, channel);

        IncomingIntegrationChannelTrait = new(client, this, channel);
        ChannelFollowerIntegrationChannelTrait = new(client, this, channel);
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestTextChannel CreateEntity(IGuildTextChannelModel model)
        => RestTextChannel.Construct(Client, Guild.Identity, model);
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
        GuildIdentity guild,
        IGuildTextChannelModel model,
        RestTextChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;
        Actor = actor ?? new(client, guild, TextChannelIdentity.Of(this));
    }

    public static RestTextChannel Construct(DiscordRestClient client, GuildIdentity guild, IGuildTextChannelModel model)
        => new(client, guild, model);

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IGuildTextChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IGuildTextChannelModel GetModel() => Model;
}
