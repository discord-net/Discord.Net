namespace Discord;

[BackLinkable]
public partial interface INestedThreadsLink : GuildThreadChannelLink.Indexable
{
    ThreadChannelLink.Paged<PagePublicArchivedThreadsParams> PublicArchivedThreads { get; }
    ThreadChannelLink.Paged<PagePrivateArchivedThreadsParams> PrivateArchivedThreads { get; }
    ThreadChannelLink.Paged<PageJoinedPrivateArchivedThreadsParams> JoinedPrivateArchivedThreads { get; }
}