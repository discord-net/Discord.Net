namespace Discord;

public interface IChannelInviteActor :
    IChannelRelationship<IInvitableChannelTrait, IInvitableChannel>,
    IActor<string, IChannelInvite>;
