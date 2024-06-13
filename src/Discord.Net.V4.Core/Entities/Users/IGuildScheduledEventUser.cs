namespace Discord;

public interface IGuildScheduledEventUser : IGuildScheduledEventUser<IGuildScheduledEventUser>;

public interface IGuildScheduledEventUser<TGuildScheduledEventUser> :
    IEntity<ulong>,
    IGuildScheduledEventUserActor<TGuildScheduledEventUser>
    where TGuildScheduledEventUser : IGuildScheduledEventUser<TGuildScheduledEventUser>;
