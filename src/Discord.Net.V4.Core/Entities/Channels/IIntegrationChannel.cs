namespace Discord;

public interface IIntegrationChannel : IIntegrationChannel<IIntegrationChannel>;
/// <summary>
///     Represents a channel in a guild that can create webhooks.
/// </summary>
public interface IIntegrationChannel<out TChannel> : IGuildChannel<TChannel>
    where TChannel : IIntegrationChannel<TChannel>
{
}
