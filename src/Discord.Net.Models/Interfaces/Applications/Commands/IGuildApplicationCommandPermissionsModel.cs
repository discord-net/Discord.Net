namespace Discord.Models;

[ModelEquality]
public partial interface IGuildApplicationCommandPermissionsModel : IEntityModel<ulong>
{
    ulong ApplicationId { get; }
    ulong GuildId { get; }
    IEnumerable<IApplicationCommandPermission> Permissions { get; }
}
