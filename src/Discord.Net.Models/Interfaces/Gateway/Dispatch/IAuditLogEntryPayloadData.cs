namespace Discord.Models;

public interface IAuditLogEntryPayloadData : IGatewayPayloadData, IAuditLogEntryModel
{
    ulong GuildId { get; }
}
