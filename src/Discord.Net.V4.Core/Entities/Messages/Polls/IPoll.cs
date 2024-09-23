using Discord.Models;

namespace Discord;

public partial interface IPoll :
    ISnowflakeEntity,
    IEntityOf<IPollModel>,
    IPollActor
{
    PollMedia Question { get; }
    
    new IReadOnlyDictionary<int, IPollAnswer> Answers { get; }
    
    DateTimeOffset? ExpiresAt { get; }
    
    bool AllowsMultipleAnswers { get; }
    
    PollLayoutType Layout { get; }
    
    PollResults? Results { get; }
}