namespace Discord.Gateway;

public interface ITransientDispatchHandler
{
    string EventName { get; }

    ValueTask ExecuteAsync(CancellationToken token);
}
