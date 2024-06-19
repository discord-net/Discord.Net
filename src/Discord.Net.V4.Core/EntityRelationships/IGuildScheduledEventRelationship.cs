namespace Discord;

public interface IGuildScheduledEventRelationship :
    IRelationship<ulong, IGuildScheduledEvent, ILoadableGuildScheduledEventActor>
{
    ILoadableGuildScheduledEventActor GuildScheduledEvent { get; }

    ILoadableGuildScheduledEventActor
        IRelationship<ulong, IGuildScheduledEvent, ILoadableGuildScheduledEventActor>.RelationshipLoadable =>
        GuildScheduledEvent;
}
