namespace Discord.Models;

public interface IGuildChannelsLink<TChannel> :
    IIndexableLink<Snowflake, IGuildChannelTrait> 
    where TChannel : IGuildChannelTrait;

public interface IGuildChannelsLink :
    IIndexableLink<Snowflake, IGuildChannelTrait>,
    IBatchLink<IGuildChannel>
{
    IGuildChannelsLink<ICategoryChannelActor> Category { get; }
    IGuildChannelsLink<IForumChannelActor> Forum { get; }
    IGuildChannelsLink<IMediaChannelActor> Media { get; }
    IGuildChannelsLink<INewsChannelActor> News { get; }
    IGuildChannelsLink<IStageChannelActor> Stage { get; }
    IGuildChannelsLink<ITextChannelActor> Text { get; }
    IGuildChannelsLink<IVoiceChannelActor> Voice { get; }
}