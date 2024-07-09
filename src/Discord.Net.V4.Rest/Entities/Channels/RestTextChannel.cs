using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Actors;
using Discord.Rest.Guilds;

namespace Discord.Rest.Channels;

using Loadable = RestLoadable<ulong, RestTextChannel, ITextChannel, IChannelModel>;

[method: TypeFactory]
public sealed partial class RestLoadableTextChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    TextChannelIdentity channel
):
    RestTextChannelActor(client, guild, channel),
    ILoadableTextChannelActor,
    IRestLoadableActor<ulong, RestTextChannel, ITextChannel, IChannelModel>
{
    [ProxyInterface(typeof(ILoadableEntity<ITextChannel>))]
    internal Loadable Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestTextChannel, IGuildTextChannelModel>(
                (_, model) => RestTextChannel.Construct(client, model, guild)
            ).Invoke
        );

    Loadable IRestLoadableActor<ulong, RestTextChannel, ITextChannel, IChannelModel>.Loadable => Loadable;
}

[ExtendInterfaceDefaults(
    typeof(ITextChannelActor),
    typeof(IModifiable<ulong, ITextChannelActor, ModifyTextChannelProperties, ModifyGuildChannelParams>)
)]
public partial class RestTextChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    TextChannelIdentity channel
):
    RestThreadableChannelActor(client, guild, channel),
    ITextChannelActor
{
    [ProxyInterface(typeof(IMessageChannelActor))]
    internal RestMessageChannelActor MessageChannelActor { get; } = new(client, channel, guild);

    public ITextChannel CreateEntity(IGuildTextChannelModel model)
        => RestTextChannel.Construct(Client, model, Guild.Identity);
}

public partial class RestTextChannel :
    RestThreadableChannel,
    ITextChannel,
    IContextConstructable<RestTextChannel, IGuildTextChannelModel, GuildIdentity, DiscordRestClient>
{
    public ILoadableEntity<ulong, ICategoryChannel>? Category => throw new NotImplementedException();

    public bool IsNsfw => Model.IsNsfw;

    public string? Topic => Model.Topic;

    public int SlowModeInterval => Model.RatelimitPerUser;

    internal override IGuildTextChannelModel Model => _model;

    [ProxyInterface(
        typeof(ITextChannelActor),
        typeof(IMessageChannelActor),
        typeof(IThreadableChannelActor),
        typeof(IEntityProvider<ITextChannel, IGuildTextChannelModel>)
    )]
    internal override RestTextChannelActor ChannelActor { get; }

    private IGuildTextChannelModel _model;

    internal RestTextChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildTextChannelModel model,
        RestTextChannelActor? actor = null
    ) : base(client, guild, model, actor)
    {
        _model = model;
        ChannelActor = actor ?? new(client, guild, TextChannelIdentity.Of(this));
    }

    public static RestTextChannel Construct(DiscordRestClient client, IGuildTextChannelModel model, GuildIdentity guild)
        => new(client, guild, model);

    public ValueTask UpdateAsync(IGuildTextChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IGuildTextChannelModel GetModel() => Model;
}
