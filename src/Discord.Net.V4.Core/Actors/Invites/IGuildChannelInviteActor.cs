namespace Discord;

public partial interface IGuildChannelInviteActor :
    IGuildInviteActor,
    IChannelInviteActor,
    IChannelRelationship<IGuildChannelActor, IGuildChannel>,
    IActor<string, IGuildChannelInvite>
{
    [SourceOfTruth]
    new IGuildChannelActor Channel { get; }

    IInvitableChannelTrait IChannelRelationship<IInvitableChannelTrait, IInvitableChannel>.Channel => Channel;
}
