namespace Discord;

public interface IGuildChannelInviteActor :
    IChannelRelationship<IInvitableChannelActor, IInvitableChannel>,
    IGuildInviteActor;
