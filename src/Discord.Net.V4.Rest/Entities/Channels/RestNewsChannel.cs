using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Guilds;

namespace Discord.Rest.Channels;

[ExtendInterfaceDefaults(typeof(INewsChannelActor))]
public partial class RestNewsChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    NewsChannelIdentity channel
) :
    RestTextChannelActor(client, guild, channel),
    INewsChannelActor
{
    [SourceOfTruth]
    [CovariantOverride]
    internal RestNewsChannel CreateEntity(IGuildNewsChannelModel model)
        => RestNewsChannel.Construct(Client, model, Guild.Identity);
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
