using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Requires the user invoking the command to have a specified role.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireRoleAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     Gets the specified Role name of the precondition.
        /// </summary>
        public string RoleName { get; }

        /// <summary>
        ///     Gets the specified Role ID of the precondition.
        /// </summary>
        public ulong? RoleId { get; }

        /// <summary>
        ///     Gets or sets the error message if the precondition
        ///     fails due to being run outside of a Guild channel.
        /// </summary>
        public string NotAGuildErrorMessage { get; set; }

        /// <summary>
        ///     Requires that the user invoking the command to have a specific Role.
        /// </summary>
        /// <param name="roleId">Id of the role that the user must have.</param>
        public RequireRoleAttribute(ulong roleId)
        {
            RoleId = roleId;
        }

        /// <summary>
        ///     Requires that the user invoking the command to have a specific Role.
        /// </summary>
        /// <param name="roleName">Name of the role that the user must have.</param>
        public RequireRoleAttribute(string roleName)
        {
            RoleName = roleName;
        }


        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is not IGuildUser guildUser)
                return Task.FromResult(PreconditionResult.FromError(NotAGuildErrorMessage ?? "Command must be used in a guild channel."));

            if (RoleId.HasValue)
            {
                if (guildUser.RoleIds.Contains(RoleId.Value))
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"User requires guild role {context.Guild.GetRole(RoleId.Value).Name}."));
            }

            if (!string.IsNullOrEmpty(RoleName))
            {
                var roleNames = guildUser.RoleIds.Select(x => guildUser.Guild.GetRole(x).Name);

                if (roleNames.Contains(RoleName))
                    return Task.FromResult(PreconditionResult.FromSuccess());
                else
                    return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"User requires guild role {RoleName}."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
