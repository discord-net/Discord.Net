using Discord.Models;

namespace Discord;

public interface IGuildChannelsLink<TChannel> :
    IIndexableLink<Snowflake, TChannel> 
    where TChannel : IGuildChannelTrait;

public interface IGuildChannelsLink :
    IIndexableLink<Snowflake, IGuildChannelTrait>,
    IBatchLink<IGuildChannel>
{
    IGuildChannelsLink<ICategoryChannelActor> Category { get; }
    IGuildChannelsLink<IForumChannelActor> Forum { get; }
    IGuildChannelsLink<IMediaChannelActor> Media { get; }
    IGuildChannelsLink<IAnnouncementChannelActor> News { get; }
    IGuildChannelsLink<IStageChannelActor> Stage { get; }
    IGuildChannelsLink<ITextChannelActor> Text { get; }
    IGuildChannelsLink<IVoiceChannelActor> Voice { get; }
}