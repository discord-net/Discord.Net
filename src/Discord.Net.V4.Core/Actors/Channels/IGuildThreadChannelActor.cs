using System.Diagnostics.CodeAnalysis;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetChannel), typeof(ThreadChannelBase)),
    Modifiable<ModifyThreadChannelProperties>(nameof(Routes.ModifyChannel)),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IGuildThreadChannelActor : 
    IThreadChannelActor,
    IGuildChannelActor;