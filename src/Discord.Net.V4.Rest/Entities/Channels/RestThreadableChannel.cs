using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;

namespace Discord.Rest;

using PublicThreadsPagedActor = RestPagedIndexableLink<
    RestThreadChannelActor,
    ulong,
    RestThreadChannel,
    IThreadChannelModel,
    ChannelThreads,
    PagePublicArchivedThreadsParams
>;

using PrivateThreadsPagedActor = RestPagedIndexableLink<
    RestThreadChannelActor,
    ulong,
    RestThreadChannel,
    IThreadChannelModel,
    ChannelThreads,
    PagePrivateArchivedThreadsParams
>;
using JoinedPrivateThreadsPagedActor = RestPagedIndexableLink<
    RestThreadChannelActor,
    ulong,
    RestThreadChannel,
    IThreadChannelModel,
    ChannelThreads,
    PageJoinedPrivateArchivedThreadsParams
>;

[ExtendInterfaceDefaults]
public partial class RestThreadableChannelActor :
    RestGuildChannelActor,
    IThreadableChannelActor,
    IRestActor<ulong, RestThreadableChannel, ThreadableChannelIdentity>
{
    [SourceOfTruth]
    public PublicThreadsPagedActor PublicArchivedThreads { get; }

    [SourceOfTruth]
    public PrivateThreadsPagedActor PrivateArchivedThreads { get; }

    [SourceOfTruth]
    public JoinedPrivateThreadsPagedActor JoinedPrivateArchivedThreads { get; }

    [SourceOfTruth]
    internal override ThreadableChannelIdentity Identity { get; }

    [TypeFactory]
    public RestThreadableChannelActor(DiscordRestClient client,
        GuildIdentity guild,
        ThreadableChannelIdentity channel) : base(client, guild, channel)
    {
        channel = Identity = channel | this;

        PublicArchivedThreads = RestActors.PublicArchivedThreads(client, guild, channel);
        PrivateArchivedThreads = RestActors.PrivateArchivedThreads(client, guild, channel);
        JoinedPrivateArchivedThreads = RestActors.JoinedPrivateArchivedThreads(client, guild, channel);
    }

    [SourceOfTruth]
    [CovariantOverride]
    internal virtual RestThreadableChannel CreateEntity(IThreadableChannelModel model)
        => RestThreadableChannel.Construct(Client, Guild.Identity, model);
}

public partial class RestThreadableChannel :
    RestGuildChannel,
    IThreadableChannel,
    IRestConstructable<RestThreadableChannel, RestThreadableChannelActor, IThreadableChannelModel>
{
    [SourceOfTruth]
    public RestCategoryChannelActor? Category { get; private set; }

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

        Category = model.ParentId.Map(
            static (id, client, guild) => new RestCategoryChannelActor(client, guild, CategoryChannelIdentity.Of(id)),
            client,
            guild
        );
    }

    public static RestThreadableChannel Construct(DiscordRestClient client,
        GuildIdentity guild,
        IThreadableChannelModel model)
    {
        return model switch
        {
            IGuildForumChannelModel guildForumChannelModel
                => RestForumChannel.Construct(client, guild, guildForumChannelModel),
            IGuildMediaChannelModel guildMediaChannelModel
                => RestMediaChannel.Construct(client, guild, guildMediaChannelModel),
            IGuildNewsChannelModel guildNewsChannelModel
                => RestNewsChannel.Construct(client, guild, guildNewsChannelModel),
            IGuildTextChannelModel guildTextChannelModel
                => RestTextChannel.Construct(client, guild, guildTextChannelModel),
            _ => new RestThreadableChannel(client, guild, model)
        };
    }

    [CovariantOverride]
    public virtual ValueTask UpdateAsync(IThreadableChannelModel model, CancellationToken token = default)
    {
        Category = Category.UpdateFrom(
            model.Id,
            RestCategoryChannelActor.Factory,
            Client,
            Actor.Guild.Identity
        );

        _model = model;

        return base.UpdateAsync(model, token);
    }

    public override IThreadableChannelModel GetModel() => Model;
}
