namespace Discord.Models;

public interface IIntegrationCreateUpdatePayloadData : IGatewayPayloadData, IIntegrationModel
{
    ulong GuildId { get; }
}
