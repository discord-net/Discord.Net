namespace Discord.Models;

public interface IGuildIntegrationsUpdatedPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
}
