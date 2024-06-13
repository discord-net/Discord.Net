namespace Discord;

public interface ILoadableGuildScheduledEventUserActor<TEventUser> :
    IGuildScheduledEventUserActor<TEventUser>,
    ILoadableEntity<ulong, TEventUser>
    where TEventUser : class, IGuildScheduledEventUser<TEventUser>;

public interface IGuildScheduledEventUserActor<TEventUser> :
    IUserRelationship,
    IMemberRelationship,
    IGuildScheduledEventRelationship
    where TEventUser : IGuildScheduledEventUser<TEventUser>;
