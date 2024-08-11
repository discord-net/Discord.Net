namespace Discord.Models;

public interface IGuildCreatePayloadData : IEntityModel<ulong>, IGatewayPayloadData
{
    bool Unavailable { get; }
}
