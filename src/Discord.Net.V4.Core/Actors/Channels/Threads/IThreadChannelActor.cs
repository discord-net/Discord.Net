using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(ThreadChannelBase)),
    Modifiable<ModifyThreadChannelProperties>(nameof(Routes.ModifyChannel)),
    RelationshipName("Thread"),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
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

    Task JoinAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Routes.JoinThread(Id), options ?? Client.DefaultRequestOptions, token);

    Task LeaveAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Routes.LeaveThread(Id), options ?? Client.DefaultRequestOptions, token);

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