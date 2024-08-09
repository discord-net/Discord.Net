namespace Discord.Gateway;

public interface ISubscribableEvent<in T> where T : Delegate
{
    void Subscribe(T handler);
    void Unsubscribe(T handler);
}
