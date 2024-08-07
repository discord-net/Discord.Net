using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AutoModerationActionExecuted : IAutoModerationActionExecutedPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("action")]
    public required AutoModerationAction Action { get; set; }

    [JsonPropertyName("rule_id")]
    public ulong RuleId { get; set; }

    [JsonPropertyName("rule_trigger_type")]
    public int RuleTriggerType { get; set; }

    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public Optional<ulong> MessageId { get; set; }

    [JsonPropertyName("alert_system_message_id")]
    public Optional<ulong> AlertSystemMessageId { get; set; }

    [JsonPropertyName("content")]
    public required string Content { get; set; }

    [JsonPropertyName("matched_keyword")]
    public Optional<string> MatchedKeyword { get; set; }

    [JsonPropertyName("matched_content")]
    public Optional<string> MatchedContent { get; set; }

    string? IAutoModerationActionExecutedPayloadData.MatchedKeyword => ~MatchedKeyword;
    string? IAutoModerationActionExecutedPayloadData.MatchedContent => ~MatchedContent;
    IAutoModerationActionModel IAutoModerationActionExecutedPayloadData.Action => Action;
    int IAutoModerationActionExecutedPayloadData.RuleTriggerType => RuleTriggerType;
    ulong? IAutoModerationActionExecutedPayloadData.ChannelId => ChannelId.ToNullable();
    ulong? IAutoModerationActionExecutedPayloadData.MessageId => MessageId.ToNullable();
    ulong? IAutoModerationActionExecutedPayloadData.AlertSystemMessageId => AlertSystemMessageId.ToNullable();
}
