namespace Discord.Models;

public interface ITeamMember : IEntityModel<ulong>
{
    int MembershipState { get; }
    ulong TeamId { get; }
    string Role { get; }
}