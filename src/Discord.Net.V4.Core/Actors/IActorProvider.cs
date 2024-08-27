namespace Discord;

public interface IActorProvider<in TClient, out TActor, in TId>
    where TClient : IDiscordClient
    where TActor : IActor<TId, IEntity<TId>>
    where TId : IEquatable<TId>
{
    internal TActor GetActor(TClient client, TId id);
}