namespace Discord;

public interface IProxied<out T>
{
    T ProxiedValue { get; }
}
