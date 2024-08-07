namespace Discord.Models;

public interface IAuditLogEntryModel : IEntityModel<ulong>
{
    string? TargetId { get; }
    IEnumerable<IAuditLogChangeModel>? Changes { get; }
    ulong? UserId { get; }
    int ActionType { get; }
    IAuditLogOptionsModel? Options { get; }
    string? Reason { get; }
}
