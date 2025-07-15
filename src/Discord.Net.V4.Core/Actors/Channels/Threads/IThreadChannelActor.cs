using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using Discord.Rest.Pipeline;

namespace Discord;

[
    RelationshipName("Thread"),
    Loadable<Routes.GetChannel>,
    Modifiable<Routes.UpdateChannel, ModifyThreadChannelProperties>,
    PagedFetchableOfMany<Routes.ListPublicArchivedThreads, PagePublicArchivedThreadsParams>,
    PagedFetchableOfMany<Routes.ListPrivateArchivedThreads, PagePrivateArchivedThreadsParams>,
    PagedFetchableOfMany<Routes.ListMyPrivateArchivedThreads, PageJoinedPrivateArchivedThreadsParams>
]
public partial interface IThreadChannelActor :
    IMessageChannelTrait,
    IActor<ulong, IThreadChannel>
{
    IThreadMemberActor
        .Enumerable
        .Indexable
        .WithCurrentMember
        .WithPagedVariant
        .BackLink<IThreadChannelActor>
        Members { get; }

    async Task JoinAsync(RequestOptions? options = null, CancellationToken token = default)
        => await Routes.JoinThread
            .Create(this)
            .AsPipeline(options)
            .RunAsync(Client, token);

    async Task LeaveAsync(RequestOptions? options = null, CancellationToken token = default)
        => await Routes.LeaveThread
            .Create(this)
            .AsPipeline(options)
            .RunAsync(Client, token);

    [LinkExtension]
    private interface WithActiveExtension
    {
        IThreadChannelActor.Enumerable.BackLink<IGuildActor> Active { get; }
    }

    [LinkExtension]
    private protected interface WithAnnouncementArchivedExtension
    {
        IAnnouncementThreadChannelActor.Paged<PagePublicArchivedThreadsParams> PublicArchived { get; }
    }

    [LinkExtension]
    private interface WithArchivedExtension
    {
        IPublicThreadChannelActor.Paged<PagePublicArchivedThreadsParams> PublicArchived { get; }
        IPrivateThreadChannelActor.Paged<PagePrivateArchivedThreadsParams> PrivateArchived { get; }
        IPrivateThreadChannelActor.Paged<PageJoinedPrivateArchivedThreadsParams> JoinedPrivateArchived { get; }
    }
}