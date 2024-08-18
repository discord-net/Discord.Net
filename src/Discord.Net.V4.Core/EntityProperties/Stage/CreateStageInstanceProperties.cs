using Discord.Models;

namespace Discord;

public sealed class CreateStageInstanceProperties : IEntityProperties<CreateStageInstanceParams>
{
    public required string Topic { get; set; }
    public Optional<StagePrivacyLevel> PrivacyLevel { get; set; }
    public Optional<bool> SendStartNotification { get; set; }
    public Optional<EntityOrId<ulong, IGuildScheduledEventActor>> ScheduledEvent { get; set; }
    
    private readonly ulong _channelId;
    
    public CreateStageInstanceProperties(EntityOrId<ulong, IStageChannelActor> channel)
    {
        _channelId = channel;
    }

    internal CreateStageInstanceProperties(IPathable path)
    {
        _channelId = path.Require<IStageChannel>();
    }
    
    public CreateStageInstanceParams ToApiModel(CreateStageInstanceParams? existing = default)
    {
        return new CreateStageInstanceParams()
        {
            Topic = Topic,
            ChannelId = _channelId,
            PrivacyLevel = PrivacyLevel.MapToInt(),
            SendStartNotification = SendStartNotification,
            GuildScheduledEventId = ScheduledEvent.MapToId(),
        };
    }
}