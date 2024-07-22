namespace Discord;

public interface IGuildScheduledEventUserActor :
    IUserRelationship,
    IMemberRelationship,
    IGuildScheduledEventRelationship,
    IGuildRelationship,
    IActor<ulong, IGuildScheduledEventUser>;
