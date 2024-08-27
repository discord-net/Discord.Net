namespace Discord;

public interface IGuildThreadsLink : GuildThreadChannelLink.Indexable.BackLink<IGuildActor>
{
    GuildThreadChannelLink.Enumerable.BackLink<IGuildActor> Active { get; }
}