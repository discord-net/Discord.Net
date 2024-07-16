using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetChannel), typeof(ThreadableChannelModelBase))]
[Modifiable<ModifyThreadableChannelProperties>(nameof(Routes.ModifyChannel))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IThreadableChannelActor :
    IGuildChannelActor,
    IActor<ulong, IThreadableChannel>
{
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PublicArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PrivateArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> JoinedPrivateArchivedThreads { get; }
}
