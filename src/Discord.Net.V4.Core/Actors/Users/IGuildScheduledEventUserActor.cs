namespace Discord;

public interface ILoadableGuildScheduledEventUserActor<TEventUser> :
    IGuildScheduledEventUserActor,
    ILoadableEntity<ulong, TEventUser>
    where TEventUser : class, IGuildScheduledEventUser;

public interface IGuildScheduledEventUserActor :
    IUserRelationship,
    IMemberRelationship,
    IGuildScheduledEventRelationship,
    IActor<ulong, IGuildScheduledEventUser>;
