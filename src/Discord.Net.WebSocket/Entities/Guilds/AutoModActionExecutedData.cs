using Discord.Rest;

namespace Discord.WebSocket;

public class AutoModActionExecutedData
{
    /// <summary>
    ///     Gets the id of the rule which action belongs to.
    /// </summary>
    public Cacheable<IAutoModRule, ulong> Rule { get; }

    /// <summary>
    ///     Gets the trigger type of rule which was triggered.
    /// </summary>
    public AutoModTriggerType TriggerType { get; }

    /// <summary>
    ///     Gets the user which generated the content which triggered the rule.
    /// </summary>
    public Cacheable<SocketGuildUser, ulong> User { get; }

    /// <summary>
    ///     Gets the channel in which user content was posted.
    /// </summary>
    public Cacheable<ISocketMessageChannel, ulong> Channel { get; }

    /// <summary>
    ///     Gets the message that triggered the action.
    /// </summary>
    /// <remarks>
    ///     This property will be <see langword="null"/> if the message was blocked by the automod.
    /// </remarks>
    public Cacheable<IUserMessage, ulong>? Message { get; }

    /// <summary>
    ///     Gets the id of the system auto moderation messages posted as a result of this action.
    /// </summary>
    /// <remarks>
    ///     This property will be <see langword="null"/> if this event does not correspond to an action
    ///     with type <see cref="AutoModActionType.SendAlertMessage"/>.
    /// </remarks>
    public ulong AlertMessageId { get; }

    /// <summary>
    ///     Gets the user-generated text content.
    /// </summary>
    /// <remarks>
    ///     This property will be empty if <see cref="GatewayIntents.MessageContent"/> is disabled.
    /// </remarks>
    public string Content { get; }

    /// <summary>
    ///     Gets the substring in content that triggered the rule.
    /// </summary>
    /// <remarks>
    ///     This property will be empty if <see cref="GatewayIntents.MessageContent"/> is disabled.
    /// </remarks>
    public string MatchedContent { get; }

    /// <summary>
    ///     Gets the word or phrase configured in the rule that triggered the rule.
    /// </summary>
    public string MatchedKeyword { get; }

    internal AutoModActionExecutedData(Cacheable<IAutoModRule, ulong> rule,
        AutoModTriggerType triggerType,
        Cacheable<SocketGuildUser, ulong> user,
        Cacheable<ISocketMessageChannel, ulong> channel,
        Cacheable<IUserMessage, ulong>? message,
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
