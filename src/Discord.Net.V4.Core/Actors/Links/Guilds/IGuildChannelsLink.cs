namespace Discord;

public interface IGuildChannelsLink : 
    GuildChannelLink.Enumerable.Indexable.BackLink<IGuildActor>
{
    TextChannelLink.Enumerable.Indexable.BackLink<IGuildActor> Text { get; }
    VoiceChannelLink.Enumerable.Indexable.BackLink<IGuildActor> Voice { get; }
    CategoryChannelLink.Enumerable.Indexable.BackLink<IGuildActor> Category { get; }
    NewsChannelLink.Enumerable.Indexable.BackLink<IGuildActor> News { get; }
    StageChannelLink.Enumerable.Indexable.BackLink<IGuildActor> Stage { get; }
    ForumChannelLink.Enumerable.Indexable.BackLink<IGuildActor> Forum { get; }
    MediaChannelLink.Enumerable.Indexable.BackLink<IGuildActor> Media { get; }
    IntegrationChannelTraitLink.Enumerable.Indexable.BackLink<IGuildActor> Integration { get; }
    ThreadableChannelLink.Enumerable.Indexable.BackLink<IGuildActor> Threadable { get; }
}