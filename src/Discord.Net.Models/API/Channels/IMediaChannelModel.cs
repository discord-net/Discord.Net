using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.GuildMedia)]
public interface IMediaChannelModel : 
    IThreadableChannelModel,
    INestedChannelModel
{
    bool IsNSFW { get; }
    
    [Range(Constants.MIN_FORUM_TOPIC_LENGTH, Constants.MAX_FORUM_TOPIC_LENGTH)]
    string? Topic { get; }
    
    [Range(Constants.MIN_RATE_LIMIT_PER_USER, Constants.MAX_RATE_LIMIT_PER_USER)]
    int RateLimitPerUser { get; }
    
    Optional<EmojiId?> DefaultReactionEmoji { get; }
    
    IReadOnlyList<ITagModel> AvailableTags { get; }
    
    SortOrderType? DefaultSortOrder { get; }
}