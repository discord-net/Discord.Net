using Discord.Models;
using Discord.Models.Json;

namespace Discord.Rest;

public sealed partial class RestNestedThreadsLink :
    RestGuildThreadChannelLink.Indexable,
    INestedThreadsLink
{
    [SourceOfTruth]
    public RestThreadChannelLink.Paged<PagePublicArchivedThreadsParams, ChannelThreads> PublicArchivedThreads { get; }

    [SourceOfTruth]
    public RestThreadChannelLink.Paged<PagePrivateArchivedThreadsParams, ChannelThreads> PrivateArchivedThreads { get; }

    [SourceOfTruth]
    public RestThreadChannelLink.Paged<
        PageJoinedPrivateArchivedThreadsParams,
        ChannelThreads
    > JoinedPrivateArchivedThreads { get; }

    public RestNestedThreadsLink(DiscordRestClient client,
        IActorProvider<DiscordRestClient, RestGuildThreadChannelActor, ulong> actorFactory,
        IPathable path
    ) : base(client, actorFactory)
    {
        PublicArchivedThreads = new(
            client,
            actorFactory,
            path,
            api => api.Threads,
            CreateFromPaged
        );
    }

    private RestThreadChannel CreateFromPaged(IThreadChannelModel model, ChannelThreads threads)
    {
        var actor = GetActor(model.Id);

        var currentMemberModel = threads.Members.FirstOrDefault(x => x.ThreadId == model.Id);
        
        if(currentMemberModel is not null)
            actor.mem
    }

    IThreadChannel IEntityProvider<IThreadChannel, IThreadChannelModel>.CreateEntity(IThreadChannelModel model)
        => CreateEntity(model);

    IGuildThreadChannelActor IActorProvider<IDiscordClient, IGuildThreadChannelActor, ulong>.GetActor(
        IDiscordClient client, ulong id
    ) => GetActor(id);

    IGuildThreadChannelActor ILinkType<
        IGuildThreadChannelActor,
        ulong,
        IThreadChannel,
        IThreadChannelModel
    >.Indexable.Specifically(ulong id)
        => Specifically(id);
}