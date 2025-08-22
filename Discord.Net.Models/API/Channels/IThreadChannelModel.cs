using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.AnnouncementThread, ChannelType.PublicThread, ChannelType.PrivateThread)]
public interface IThreadChannelModel : 
    IGuildChannelModel,
    INestedChannelModel
{
    new Snowflake ParentId { get; }
    
    int MemberCount { get; }
    
    Optional<int> MessageCount { get; }
    
    Optional<IReadOnlyList<Snowflake>> AppliedTags { get; }
    
    Snowflake OwnerId { get; }
    
    bool IsNSFW { get; }
    
    [Range(Constants.MIN_TOPIC_LENGTH, Constants.MAX_TOPIC_LENGTH)]
    string? Topic { get; }
    
    [Range(Constants.MIN_RATE_LIMIT_PER_USER, Constants.MAX_RATE_LIMIT_PER_USER)]
    Optional<int> RateLimitPerUser { get; }

    Optional<Snowflake?> INestedChannelModel.ParentId => ParentId;
}