namespace Discord.Models;

public interface ITeamModel : IEntityModel<ulong>
{
    string? Icon { get; }
    IEnumerable<ITeamMember> Members { get; }
    string Name { get; }
    ulong OwnerId { get; }
}