using Discord.Rest;

namespace Discord.WebSocket;

public class AutoModActionExecutedData
{
    public Cacheable<IAutoModRule, ulong> Rule { get; }

    public AutoModTriggerType TriggerType { get; }

    public Cacheable<SocketGuildUser, ulong> User { get; }

    public Cacheable<ISocketMessageChannel, ulong> Channel { get; }

    public Cacheable<IUserMessage, ulong> Message { get; }

    public ulong AlertMessageId { get; }
    
    public string Content { get; }

    public string MatchedContent { get; }

    public string MatchedKeyword { get; }

    internal AutoModActionExecutedData(Cacheable<IAutoModRule, ulong> rule,
        AutoModTriggerType triggerType,
        Cacheable<SocketGuildUser, ulong> user,
        Cacheable<ISocketMessageChannel, ulong> channel,
        Cacheable<IUserMessage, ulong> message,
        ulong alertMessageId,
        string content,
        string matchedContent,
        string matchedKeyword
    )
    {
        Rule = rule;
        TriggerType = triggerType;
        User = user;
        Channel = channel;
        Message = message;
        AlertMessageId = alertMessageId;
        Content = content;
        MatchedContent = matchedContent;
        MatchedKeyword = matchedKeyword;
    }
}
