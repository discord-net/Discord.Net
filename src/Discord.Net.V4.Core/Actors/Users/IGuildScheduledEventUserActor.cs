namespace Discord;

public interface IGuildScheduledEventUserActor :
    IUserRelationship,
    IMemberRelationship,
    IGuildScheduledEventRelationship,
    IActor<ulong, IGuildScheduledEventUser>;
