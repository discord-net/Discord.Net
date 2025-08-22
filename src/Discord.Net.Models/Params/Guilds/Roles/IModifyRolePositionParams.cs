namespace Discord.Models;

public interface IModifyRolePositionParams
{
    ulong Id { get; }
    Optional<int?> Position { get; }
}