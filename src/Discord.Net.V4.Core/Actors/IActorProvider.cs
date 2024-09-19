namespace Discord;

public interface IActorProvider<out TActor, in TId>
    where TActor : IActor<TId, IEntity<TId>>
    where TId : IEquatable<TId>
{
    internal TActor GetActor(TId id);
}