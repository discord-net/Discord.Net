namespace Discord.Models;

public interface IVoiceServerUpdatedPayloadData : IGatewayPayloadData
{
    string Token { get; }
    ulong GuildId { get; }
    string? Endpoint { get; }
}
