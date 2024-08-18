using Discord.Models;
using Discord.Rest;

namespace Discord;

[Trait]
public partial interface IInvitableChannelTrait :
    IChannelActor,
    IEntityProvider<IInvitableChannel, IChannelModel>,
    IActorTrait<ulong, IInvitableChannel>,
    IInvitable<CreateChannelInviteProperties>
{
    [SourceOfTruth]
    internal new IInvitableChannel CreateEntity(IChannelModel model);
}
