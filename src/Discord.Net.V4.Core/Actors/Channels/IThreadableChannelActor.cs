using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;
using Discord;

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
    new INestedThreadsLink Threads { get; }
}