using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Trait,
    Loadable<Routes.GetChannel>,
    Modifiable<Routes.UpdateChannel, ModifyThreadableChannelProperties>,
]
public partial interface IThreadableChannelTrait :
    IGuildChannelActor,
    IInvitableTrait<IGuildChannelInviteActor, IGuildChannelInvite>,
    IHasThreadsTrait,
    IActorTrait<ulong, IThreadableChannel>;

[Trait]
public partial interface IThreadableChannelTrait<TLink> :
    IThreadableChannelTrait,
    IHasThreadsTrait<TLink>
    where TLink : class, IThreadChannelActor.Indexable;