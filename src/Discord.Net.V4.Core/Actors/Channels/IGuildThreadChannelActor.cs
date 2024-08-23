using System.Diagnostics.CodeAnalysis;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(ThreadChannelBase)),
    Modifiable<ModifyThreadChannelProperties>(nameof(Routes.ModifyChannel)),
    Creatable<CreateThreadFromMessageProperties>(
        nameof(Routes.StartThreadFromMessage),
        nameof(IThreadableChannelActor.Threads),
        MethodName = "CreateFromMessageAsync"
    ),
    Creatable<CreateThreadWithoutMessageProperties>(
        nameof(Routes.StartThreadWithoutMessage),
        nameof(IThreadableChannelActor.Threads),
        MethodName = "CreateAsync"
    ),
    Creatable<CreateThreadInForumOrMediaProperties>(
        nameof(Routes.StartThreadInForum),
        nameof(IForumChannelActor.Threads),
        nameof(IMediaChannelActor.Threads),
        MethodName = "CreateAsync"
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IGuildThreadChannelActor : 
    IThreadChannelActor,
    IGuildChannelActor;