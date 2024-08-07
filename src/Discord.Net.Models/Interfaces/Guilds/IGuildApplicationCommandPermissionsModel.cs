namespace Discord.Models;

public interface IGuildApplicationCommandPermissionsModel : IEntityModel<ulong>
{
    ulong ApplicationId { get; }
    ulong GuildId { get; }
    IEnumerable<IApplicationCommandPermission> Permissions { get; }
}
