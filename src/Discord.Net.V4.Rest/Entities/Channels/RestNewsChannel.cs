using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;

namespace Discord.Rest.Channels;

[method: TypeFactory]
[ExtendInterfaceDefaults(typeof(INewsChannelActor))]
public partial class RestNewsChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    NewsChannelIdentity channel
) :
    RestTextChannelActor(client, guild, channel),
    INewsChannelActor,
    IRestActor<ulong, RestNewsChannel, NewsChannelIdentity>
{
    public override NewsChannelIdentity Identity { get; } = channel;

    [SourceOfTruth]
    [CovariantOverride]
    internal RestNewsChannel CreateEntity(IGuildNewsChannelModel model)
        => RestNewsChannel.Construct(Client, Guild.Identity, model);
}

public partial class RestNewsChannel :
    RestTextChannel,
    INewsChannel,
    IContextConstructable<RestNewsChannel, IGuildNewsChannelModel, GuildIdentity, DiscordRestClient>
{
    internal override IGuildNewsChannelModel Model => _model;

    [ProxyInterface(
        typeof(INewsChannelActor),
        typeof(IEntityProvider<INewsChannel, IGuildNewsChannelModel>)
    )]
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

    public static RestNewsChannel Construct(DiscordRestClient client, GuildIdentity guild, IGuildNewsChannelModel model)
        => new(client, guild, model);

    [CovariantOverride]
    public ValueTask UpdateAsync(IGuildNewsChannelModel model, CancellationToken token = default)
    {
        _model = model;
        return base.UpdateAsync(model, token);
    }

    public override IGuildNewsChannelModel GetModel() => Model;
}
