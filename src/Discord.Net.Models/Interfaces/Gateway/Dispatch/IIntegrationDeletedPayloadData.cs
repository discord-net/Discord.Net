namespace Discord.Models;

public interface IIntegrationDeletedPayloadData : IGatewayPayloadData
{
    ulong Id { get; }
    ulong GuildId { get; }
    ulong ApplicationId { get; }
}
