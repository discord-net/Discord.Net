namespace Discord.Models;

public interface IGuildRoleDeletePayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    ulong RoleId { get; }
}
