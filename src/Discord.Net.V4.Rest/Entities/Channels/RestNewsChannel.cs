using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;

namespace Discord.Rest.Channels;

public partial class RestLoadableNewsChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    NewsChannelIdentity channel
):
    RestNewsChannelActor(client, guild, channel),
    ILoadableNewsChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<INewsChannel>))]
    internal RestLoadable<ulong, RestNewsChannel, INewsChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestNewsChannel, IGuildNewsChannelModel>(
                (_, model) => RestNewsChannel.Construct(client, model, guild)
            ).Invoke
        );
}

[ExtendInterfaceDefaults(typeof(INewsChannelActor))]
public partial class RestNewsChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    NewsChannelIdentity channel
):
    RestTextChannelActor(client, guild, channel),
    INewsChannelActor;

public partial class RestNewsChannel :
    RestTextChannel,
    INewsChannel,
    IContextConstructable<RestNewsChannel, IGuildNewsChannelModel, GuildIdentity, DiscordRestClient>
{
    internal override IGuildNewsChannelModel Model => _model;

    internal override RestNewsChannelActor Actor { get; }

    private IGuildNewsChannelModel _model;

    internal RestNewsChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IGuildNewsChannelModel model,
        RestNewsChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;

        Actor = actor ?? new(client, guild, NewsChannelIdentity.Of(this));
    }

    public static RestNewsChannel Construct(DiscordRestClient client, IGuildNewsChannelModel model, GuildIdentity guild)
        => new(client, guild, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildNewsChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IGuildNewsChannelModel GetModel() => Model;
}
