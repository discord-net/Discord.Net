using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessageReferenceModel : IModel
{
    Optional<MessageReferenceType> Type { get; }
    
    Optional<Snowflake> MessageId { get; }
    
    Optional<Snowflake> ChannelId { get; }
    
    Optional<Snowflake> GuildId { get; }
    
    Optional<bool> FailIfNotExists { get; }
}

public enum MessageReferenceType
{
    Default = 0,
    Forward = 1
}