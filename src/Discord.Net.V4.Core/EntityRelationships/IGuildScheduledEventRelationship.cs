namespace Discord;

public interface IGuildScheduledEventRelationship : IGuildScheduledEventRelationship<IGuildScheduledEvent>;
public interface IGuildScheduledEventRelationship<TEvent> :
    IRelationship<ulong, TEvent, ILoadableGuildScheduledEventActor<TEvent>>
    where TEvent : class, IGuildScheduledEvent
{
    ILoadableGuildScheduledEventActor<TEvent> GuildScheduledEvent { get; }

    ILoadableGuildScheduledEventActor<TEvent>
        IRelationship<ulong, TEvent, ILoadableGuildScheduledEventActor<TEvent>>.RelationshipLoadable =>
        GuildScheduledEvent;
}
