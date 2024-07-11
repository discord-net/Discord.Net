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
    [ProxyInterface(typeof(ILoadableEntity<IGuildChannel>))]
    internal RestLoadable<ulong, RestGuildChannel, IGuildChannel, IChannelModel> Loadable { get; } =
        new(
            client,
            channel,
            Routes.GetChannel(channel.Id),
            EntityUtils.FactoryOfDescendantModel<ulong, IChannelModel, RestGuildChannel, IThreadableChannelModel>(
                (_, model) => RestGuildChannel.Construct(client, model, guild)
            ).Invoke
        );
}

public partial class RestThreadableChannelActor(
    DiscordRestClient client,
    GuildIdentity guild,
    ThreadableChannelIdentity channel
) :
    RestGuildChannelActor(client, guild, channel),
    IThreadableChannelActor
{
    public ThreadsPagedActor PublicArchivedThreads { get; }
        = RestActors.PublicArchivedThreads(client, guild, channel);

    public ThreadsPagedActor PrivateArchivedThreads { get; }
        = RestActors.PrivateArchivedThreads(client, guild, channel);

    public ThreadsPagedActor JoinedPrivateArchivedThreads { get; }
        = RestActors.JoinedPrivateArchivedThreads(client, guild, channel);

    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> IThreadableChannelActor.PublicArchivedThreads
        => PublicArchivedThreads;

    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> IThreadableChannelActor.PrivateArchivedThreads
        => PrivateArchivedThreads;

    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> IThreadableChannelActor.JoinedPrivateArchivedThreads
        => JoinedPrivateArchivedThreads;
}

public partial class RestThreadableChannel :
    RestGuildChannel,
    IThreadableChannel,
    IContextConstructable<RestThreadableChannel, IThreadableChannelModel, GuildIdentity, DiscordRestClient>
{
    public int? DefaultThreadSlowmode => Model.DefaultThreadRateLimitPerUser;

    public ThreadArchiveDuration DefaultArchiveDuration => (ThreadArchiveDuration)Model.DefaultAutoArchiveDuration;

    internal override IThreadableChannelModel Model { get; }

    internal override RestThreadableChannelActor Actor { get; }

    internal RestThreadableChannel(
        DiscordRestClient client,
        GuildIdentity guild,
        IThreadableChannelModel model,
        RestThreadableChannelActor? actor = null
    ) : base(client, guild, model)
    {
        Model = model;
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

    public override IThreadableChannelModel GetModel() => Model;
}
