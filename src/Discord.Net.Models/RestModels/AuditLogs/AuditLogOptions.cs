using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class AuditLogOptions : IAuditLogOptionsModel
{
    [JsonPropertyName("count")]
    public Optional<string> Count { get; set; }

    [JsonPropertyName("channel_id")]
    public Optional<ulong> ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public Optional<ulong> MessageId { get; set; }

    //Prune
    [JsonPropertyName("delete_member_days")]
    public Optional<string> DeleteMemberDays { get; set; }

    [JsonPropertyName("members_removed")]
    public Optional<string> MembersRemoved { get; set; }

    //Overwrite
    [JsonPropertyName("role_name")]
    public Optional<string> RoleName { get; set; }

    [JsonPropertyName("type")]
    public Optional<string> Type { get; set; }

    [JsonPropertyName("id")]
    public Optional<ulong> OverwriteTargetId { get; set; }

    // App command perm update
    [JsonPropertyName("application_id")]
    public Optional<ulong> ApplicationId { get; set; }

    // AutoMod
    [JsonPropertyName("auto_moderation_rule_name")]
    public Optional<string> AutoModerationRuleName { get; set; }

    [JsonPropertyName("auto_moderation_rule_trigger_type")]
    public Optional<string> AutoModerationRuleTriggerType { get; set; }

    [JsonPropertyName("integration_type")]
    public Optional<string> IntegrationType { get; set; }

    ulong? IAuditLogOptionsModel.ChannelId => ~ChannelId;

    string? IAuditLogOptionsModel.Count => ~Count;

    string? IAuditLogOptionsModel.DeleteMemberDays => ~DeleteMemberDays;

    ulong? IAuditLogOptionsModel.Id => ~OverwriteTargetId;

    string? IAuditLogOptionsModel.MembersRemoved => ~MembersRemoved;

    ulong? IAuditLogOptionsModel.MessageId => ~MessageId;

    string? IAuditLogOptionsModel.RoleName => ~RoleName;

    string? IAuditLogOptionsModel.Type => ~Type;

    string? IAuditLogOptionsModel.IntegrationType => ~IntegrationType;
    ulong? IAuditLogOptionsModel.ApplicationId => ~ApplicationId;

    string? IAuditLogOptionsModel.AutoModerationRuleName => ~AutoModerationRuleName;

    string? IAuditLogOptionsModel.AutoModerationRuleTriggerType => ~AutoModerationRuleTriggerType;
}
