namespace Discord;

public interface IChannelInviteActor :
    IChannelRelationship<IInvitableChannelActor, IInvitableChannel>,
    IActor<string, IChannelInvite>;
