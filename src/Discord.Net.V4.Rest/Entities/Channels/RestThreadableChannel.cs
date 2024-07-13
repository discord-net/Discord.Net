using Discord.Models;
using Discord.Models.Json;
using Discord.Rest.Channels;

namespace Discord.Rest.Channels;

using ThreadsPagedActor = RestPagedActor<ulong, RestThreadChannel, ChannelThreads, PageThreadChannelsParams>;

public partial class RestLoadableThreadableChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadableChannelIdentity channel
) :
    RestThreadableChannelActor(client, guild, channel),
    ILoadableThreadableChannelActor
{
    [ProxyInterface(typeof(ILoadableEntity<IThreadableChannel>))]
    internal RestLoadable<ulong, RestThreadableChannel, IThreadableChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestThreadableChannel, IThreadableChannelModel>(
                (_, model) => RestThreadableChannel.Construct(client, model, guild)
            ).Invoke
        );
}

public partial class RestThreadableChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadableChannelIdentity channel
) :
    RestGuildChannelActor(client, guild, channel),
    IThreadableChannelActor,
    IActor<ulong, RestThreadableChannel>
{
    [SourceOfTruth]
    public ThreadsPagedActor PublicArchivedThreads { get; }
        = RestActors.PublicArchivedThreads(client, guild, channel);

    [SourceOfTruth]
    public ThreadsPagedActor PrivateArchivedThreads { get; }
        = RestActors.PrivateArchivedThreads(client, guild, channel);

    [SourceOfTruth]
    public ThreadsPagedActor JoinedPrivateArchivedThreads { get; }
        = RestActors.JoinedPrivateArchivedThreads(client, guild, channel);

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestThreadableChannel CreateEntity(IThreadableChannelModel model)
        => RestThreadableChannel.Construct(Client, model, Guild.Identity);
}

public partial class RestThreadableChannel :
    RestGuildChannel,
    IThreadableChannel,
    IContextConstructable<RestThreadableChannel, IThreadableChannelModel, GuildIdentity, DiscordRestClient>
{
    public int? DefaultThreadSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ThreadArchiveDuration DefaultArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    internal override IThreadableChannelModel Model => _model;

    [ProxyInterface(
        typeof(IThreadableChannelActor),
        typeof(IEntityProvider<IThreadableChannel, IThreadableChannelModel>)
    )]
    internal override RestThreadableChannelActor Actor { get; }

    private IThreadableChannelModel _model;

    internal RestThreadableChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IThreadableChannelModel model,
        RestThreadableChannelActor? actor = null
    ) : base(client, guild, model)
    {
        _model = model;
        Actor = actor ?? new(
            client,
            guild,
            ThreadableChannelIdentity.Of(this)
        );
    }

    public static RestThreadableChannel Construct(
        DiscordRestClient client,
        IThreadableChannelModel model,
        GuildIdentity guild)
    {
        return model switch
        {
            IGuildForumChannelModel guildForumChannelModel
                => RestForumChannel.Construct(client, guildForumChannelModel, guild),
            IGuildMediaChannelModel guildMediaChannelModel
                => RestMediaChannel.Construct(client, guildMediaChannelModel, guild),
            IGuildNewsChannelModel guildNewsChannelModel
                => RestNewsChannel.Construct(client, guildNewsChannelModel, guild),
            IGuildTextChannelModel guildTextChannelModel
                => RestTextChannel.Construct(client, guildTextChannelModel, guild),
            _ => new RestThreadableChannel(client, guild, model)
        };
    }

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IThreadableChannelModel model, CancellationToken token = default)
    {
        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IThreadableChannelModel GetModel() => Model;
}
