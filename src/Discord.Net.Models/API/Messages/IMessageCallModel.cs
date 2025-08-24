using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessageCallModel : IModel
{
    IReadOnlyList<Snowflake> Participants { get; }
    
    Optional<DateTimeOffset?> EndedTimestamp { get; }
}