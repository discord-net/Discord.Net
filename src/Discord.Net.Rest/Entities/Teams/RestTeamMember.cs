using System;
using Model = Discord.API.TeamMember;

namespace Discord.Rest
{
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

        internal RestTeamMember(BaseDiscordClient discord, Model model)
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
        }
    }
}
