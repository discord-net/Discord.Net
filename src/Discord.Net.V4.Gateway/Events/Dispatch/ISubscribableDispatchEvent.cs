namespace Discord.Gateway.Dispatch;

public interface ISubscribableDispatchEvent<in T> :
    ISubscribableEvent<T>
    where T : Delegate
{
    static abstract string DispatchEventName { get; }
}
