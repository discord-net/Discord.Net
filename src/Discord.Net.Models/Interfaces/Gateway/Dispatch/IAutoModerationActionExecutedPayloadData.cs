namespace Discord.Models;

public interface IAutoModerationActionExecutedPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    IAutoModerationActionModel Action { get; }
    ulong RuleId { get; }
    int RuleTriggerType { get; }
    ulong UserId { get; }
    ulong? ChannelId { get; }
    ulong? MessageId { get; }
    ulong? AlertSystemMessageId { get; }
    string Content { get; }
    string? MatchedKeyword { get; }
    string? MatchedContent { get; }
}
