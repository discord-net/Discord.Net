using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(ThreadableChannelBase))]
[Modifiable<ModifyThreadableChannelProperties>(nameof(Routes.ModifyChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IThreadableChannelActor :
    IGuildChannelActor,
    IActor<ulong, IThreadableChannel>
{
    PagedIndexablePublicArchivedThreadsLink PublicArchivedThreads { get; }
    PagedIndexablePrivateArchivedThreadsLink PrivateArchivedThreads { get; }
    PagedIndexableJoinedPrivateArchivedThreadsLink JoinedPrivateArchivedThreads { get; }
}
