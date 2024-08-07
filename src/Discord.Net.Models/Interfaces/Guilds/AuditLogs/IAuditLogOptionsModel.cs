namespace Discord.Models;

public interface IAuditLogOptionsModel
{
    ulong? ApplicationId { get; }
    string? AutoModerationRuleName { get; }
    string? AutoModerationRuleTriggerType { get; }
    ulong? ChannelId { get; }
    string? Count { get; }
    string? DeleteMemberDays { get; }
    ulong? Id { get; }
    string? MembersRemoved { get; }
    ulong? MessageId { get; }
    string? RoleName { get; }
    string? Type { get; }
    string? IntegrationType { get; }
}
