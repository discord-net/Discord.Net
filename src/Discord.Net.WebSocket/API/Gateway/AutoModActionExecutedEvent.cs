using Newtonsoft.Json;
namespace Discord.API.Gateway;

internal class AutoModActionExecutedEvent
{
    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("action")]
    public Discord.API.AutoModAction Action { get; set; }

    [JsonProperty("rule_id")]
    public ulong RuleId { get; set; }

    [JsonProperty("rule_trigger_type")]
    public AutoModTriggerType TriggerType { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonProperty("message_id")]
    public Optional<ulong> MessageId { get; set; }

    [JsonProperty("alert_system_message_id")]
    public Optional<ulong> AlertSystemMessageId { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("matched_keyword")]
    public Optional<string> MatchedKeyword { get; set; }

    [JsonProperty("matched_content")]
    public Optional<string> MatchedContent { get; set; }
}
