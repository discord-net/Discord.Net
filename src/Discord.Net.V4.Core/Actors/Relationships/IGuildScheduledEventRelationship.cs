namespace Discord;

public interface IGuildScheduledEventRelationship :
    IRelationship<IGuildScheduledEventActor, ulong, IGuildScheduledEvent>
{
    IGuildScheduledEventActor GuildScheduledEvent { get; }

    IGuildScheduledEventActor IRelationship<IGuildScheduledEventActor, ulong, IGuildScheduledEvent>.RelationshipActor
        => GuildScheduledEvent;
}
