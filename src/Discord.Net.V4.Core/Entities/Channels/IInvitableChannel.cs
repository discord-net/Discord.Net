namespace Discord;

public interface IInvitableChannel : IInvitableChannel<IInvitableChannel>;

/// <summary>
///     Represents a channel that can contain invites.
/// </summary>
public interface IInvitableChannel<out TChannel> : IGuildChannel<TChannel>
    where TChannel : IInvitableChannel<TChannel>
{
}
