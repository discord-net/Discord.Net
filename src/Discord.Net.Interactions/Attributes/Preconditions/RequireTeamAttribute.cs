using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Requires the command to be invoked by a member of the team that owns the bot.
    /// </summary>
    /// <remarks>
    ///     This precondition will restrict the access of the command or module to the a member of the team of the Discord application, narrowed to specific team roles if specified.
    ///     If the precondition fails to be met, an erroneous <see cref="PreconditionResult"/> will be returned with the
    ///     message "Command can only be run by a member of the bot's team."
    ///     <note>
    ///     This precondition will only work if the account has a <see cref="TokenType"/> of <see cref="TokenType.Bot"/>
    ///     ;otherwise, this precondition will always fail.
    ///     </note>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireTeamAttribute : PreconditionAttribute
    {
        /// <summary>
        ///      The team roles to require. Valid values: "*", "admin", "developer", or "read_only"
        /// </summary>
        public string[] TeamRoles { get; } = [];

        /// <summary>
        ///     Requires that the user invoking the command to have a specific team role.
        /// </summary>
        /// <param name="teamRoles">The team roles to require. Valid values: "*", "admin", "developer", or "read_only"</param>
        public RequireTeamAttribute(params string[] teamRoles)
        {
            TeamRoles = teamRoles ?? TeamRoles;
        }

        /// <inheritdoc />
        public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo command, IServiceProvider services)
        {
            switch (context.Client.TokenType)
            {
                case TokenType.Bot:
                    var application = await context.Client.GetApplicationInfoAsync().ConfigureAwait(false);

                    var idFound = false;

                    foreach (var member in application.Team.TeamMembers)
                    {
                        if (member.User.Id == context.User.Id)
                        {
                            if (TeamRoles.Length == 0 || TeamRoles.Any(role => member.Permissions.Contains(role)))
                            {
                                idFound = true;
                            }

                            break;
                        }
                    }

                    if (idFound == false)
                        return PreconditionResult.FromError(ErrorMessage ?? $"Command can only be run by a member of the bot's team {(TeamRoles.Length == 0 ? "." : "with the specified permissions.")}");
                    return PreconditionResult.FromSuccess();
                default:
                    return PreconditionResult.FromError($"{nameof(RequireTeamAttribute)} is not supported by this {nameof(TokenType)}.");
            }
        }
    }
}
