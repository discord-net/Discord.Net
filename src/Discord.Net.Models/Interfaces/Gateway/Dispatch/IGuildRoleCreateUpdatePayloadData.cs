namespace Discord.Models;

public interface IGuildRoleCreateUpdatePayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    IRoleModel Role { get; }
}
