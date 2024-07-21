using System;

using Model = Discord.API.TeamMember;
using TeamModel = Discord.API.Team;

namespace Discord.Rest;

public class RestTeamMember : ITeamMember
{
    /// <inheritdoc />
    public MembershipState MembershipState { get; }

    /// <inheritdoc />
    public string[] Permissions { get; }

    /// <inheritdoc />
    public ulong TeamId { get; }

    /// <inheritdoc />
    public IUser User { get; }

    /// <inheritdoc />
    public TeamRole Role { get; }

    internal RestTeamMember(BaseDiscordClient discord, Model model, TeamModel team)
    {
        MembershipState = model.MembershipState switch
        {
            API.MembershipState.Invited => MembershipState.Invited,
            API.MembershipState.Accepted => MembershipState.Accepted,
            _ => throw new InvalidOperationException("Invalid membership state"),
        };
        Permissions = model.Permissions;
        TeamId = model.TeamId;
        User = RestUser.Create(discord, model.User);

        if (team.OwnerUserId == model.User.Id)
            Role = TeamRole.Owner;
        else
            Role = model.Role switch
            {
                "admin" => TeamRole.Admin,
                "developer" => TeamRole.Developer,
                "read_only" => TeamRole.ReadOnly,
                _ => 0
            };
    }
}
