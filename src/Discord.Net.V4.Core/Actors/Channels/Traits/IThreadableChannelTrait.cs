using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Trait, 
    Loadable(nameof(Routes.GetChannel), typeof(ThreadableChannelBase)),
    Modifiable<ModifyThreadableChannelProperties>(nameof(Routes.ModifyChannel)),
]
public partial interface IThreadableChannelTrait<TLink> :
    IGuildChannelActor,
    IInvitableTrait<IGuildChannelInviteActor, IGuildChannelInvite>,
    IHasThreadsTrait<
        IThreadChannelActor,
        TLink
    >,
    IActorTrait<ulong, IThreadableChannel>
    where TLink : class, IThreadChannelActor.Indexable;