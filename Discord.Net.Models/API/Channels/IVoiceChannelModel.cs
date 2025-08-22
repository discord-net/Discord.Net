using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
[Variant(nameof(Type), ChannelType.GuildVoice)]
public interface IVoiceChannelModel : IGuildChannelModel, INestedChannelModel
{
    bool IsNSFW { get; }
    
    [Range(Constants.MIN_TOPIC_LENGTH, Constants.MAX_TOPIC_LENGTH)]
    string? Topic { get; }
    
    [Range(Constants.MIN_RATE_LIMIT_PER_USER, Constants.MAX_RATE_LIMIT_PER_USER)]
    int RateLimitPerUser { get; }
    
    int Bitrate { get; }
    
    Optional<int> UserLimit { get; }
}