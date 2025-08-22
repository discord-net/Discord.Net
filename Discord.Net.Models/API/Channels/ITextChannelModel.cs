using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.GuildText)]
public interface ITextChannelModel : 
    IThreadableChannelModel, 
    INestedChannelModel
{
    bool IsNSFW { get; }
    
    [Range(Constants.MIN_TOPIC_LENGTH, Constants.MAX_TOPIC_LENGTH)]
    string? Topic { get; }
    
    [Range(Constants.MIN_RATE_LIMIT_PER_USER, Constants.MAX_RATE_LIMIT_PER_USER)]
    Optional<int> RateLimitPerUser { get; }
}