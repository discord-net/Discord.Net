namespace Discord.Gateway;

public interface IInvocableEventHandler
{
    ValueTask InvokeAsync(CancellationToken token = default);
}
