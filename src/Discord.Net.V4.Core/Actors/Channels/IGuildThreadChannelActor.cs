using System.Diagnostics.CodeAnalysis;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(ThreadChannelBase)),
    Modifiable<ModifyThreadChannelProperties>(nameof(Routes.ModifyChannel)),
    Creatable<CreateThreadFromMessageProperties>(
        nameof(Routes.StartThreadFromMessage),
        nameof(IThreadableChannelActor),
        MethodName = "CreateFromMessageAsync"
    ),
    Creatable<CreateThreadWithoutMessageProperties>(
        nameof(Routes.StartThreadWithoutMessage),
        nameof(IThreadableChannelActor),
        MethodName = "CreateAsync"
    ),
    Creatable<CreateThreadInForumOrMediaProperties>(
        nameof(Routes.StartThreadInForum),
        nameof(IForumChannelActor),
        nameof(IMediaChannelActor),
        MethodName = "CreateAsync"
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IGuildThreadChannelActor :
    IThreadChannelActor,
    IGuildRelationship
{
    [SourceOfTruth]
    new IGuildThreadMemberActor.Enumerable.Indexable.WithCurrentMember.BackLink<IGuildThreadChannelActor> Members { get; }

    [LinkExtension]
    private interface WithActiveExtension
    {
        IGuildThreadChannelActor.Enumerable Active { get; }
    }

    [LinkExtension]
    private interface WithNestedThreadsExtension
    {
        IGuildThreadChannelActor.Paged<PagePublicArchivedThreadsParams> PublicArchivedThreads { get; }
        IGuildThreadChannelActor.Paged<PagePrivateArchivedThreadsParams> PrivateArchivedThreads { get; }
        IGuildThreadChannelActor.Paged<PageJoinedPrivateArchivedThreadsParams> JoinedPrivateArchivedThreads { get; }
    }
}