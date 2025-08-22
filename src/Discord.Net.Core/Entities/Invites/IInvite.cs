namespace Discord;

public interface IInvite : IEntity<InviteId>;

public interface IGuildInvite : IInvite;
public interface IChannelInvite : IInvite;

public interface IGuildChannelInvite : IGuildInvite, IChannelInvite;