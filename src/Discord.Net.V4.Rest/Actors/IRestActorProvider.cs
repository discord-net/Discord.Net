namespace Discord.Rest;

public interface IRestActorProvider<out TActor, in TId> :
    IActorProvider<DiscordRestClient, TActor, TId>
    where TActor : IActor<TId, IEntity<TId>>
    where TId : IEquatable<TId>;