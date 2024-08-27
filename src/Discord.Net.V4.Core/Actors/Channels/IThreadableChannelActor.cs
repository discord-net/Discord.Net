using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using Discord.Paging;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(ThreadableChannelBase)),
    Modifiable<ModifyThreadableChannelProperties>(nameof(Routes.ModifyChannel)),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IThreadableChannelActor :
    IGuildChannelActor,
    IInvitableTrait<IGuildChannelInviteActor, IGuildChannelInvite>,
    IContainsThreadsTrait<IGuildThreadChannelActor>,
    IActor<ulong, IThreadableChannel>
{
    [SourceOfTruth]
    new ThreadsLink Threads { get; }
    
    [BackLinkable]
    public partial interface ThreadsLink : GuildThreadChannelLink.Indexable
    {
        ThreadChannelLink.Paged<PagePublicArchivedThreadsParams> PublicArchivedThreads { get; }
        ThreadChannelLink.Paged<PagePrivateArchivedThreadsParams> PrivateArchivedThreads { get; }
        ThreadChannelLink.Paged<PageJoinedPrivateArchivedThreadsParams> JoinedPrivateArchivedThreads { get; }
    }
}