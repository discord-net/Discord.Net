using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Ensures that command parameters are passed within a correct hierarchical context.
    /// </summary>
    /// <example>
    ///     Here's how it might be used inside the commands:
    /// <code>
    /// <![CDATA[
    /// [RequireContext(ContextType.Guild)]
    /// [RequireBotPermission(GuildPermission.ManageRoles)]
    /// [RequireUserPermission(GuildPermission.ManageRoles)]
    /// [Command("role")]
    /// public async Task RoleAsync(IGuildUser user, [DoHierarchyCheck] IRole role)
    /// {
    ///     if (user.RoleIds.Any(id => id == role.Id))
    ///     {
    ///         await ReplyAsync($"{user.Mention} already has {role.Mention}!");
    ///         return;
    ///     }
    /// 
    ///     await user.AddRoleAsync(role);
    ///     await ReplyAsync($"Added {role.Mention} to {user.Mention}.");
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <remarks>
    ///     This is useful when you need to perform hierarchical operations within a guild, such as managing roles or users.
    ///     <note type="warning">
    ///         This works only for <see cref="IRole"/>, <see cref="IGuildUser"/>, and <see cref="IUser"/>.
    ///     </note>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the parameter type is not supported by this precondition attribute.
    /// </exception>
    /// <seealso cref="RequireBotPermissionAttribute"/>
    /// <seealso cref="RequireUserPermissionAttribute"/>
    public class DoHierarchyCheckAttribute : ParameterPreconditionAttribute
    {
        /// <summary>
        ///     Gets or sets the error message displayed when the command is used outside of a guild.
        /// </summary>
        public string NotAGuildErrorMessage { get; set; } = "This command cannot be used outside of a guild.";

        /// <summary>
        ///     Gets the error message to be returned if execution context doesn't pass the precondition check.
        /// </summary>
        public string ErrorMessage { get; set; } = "You cannot target anyone else whose hierarchy are higher or equal to yours or the bot's.";

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when the parameter type is not supported by this precondition attribute.
        /// </exception>
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (context.User is not IGuildUser user)
                return PreconditionResult.FromError(NotAGuildErrorMessage);

            var hieararchy = GetTargetHieararchy(context, value);
            if (hieararchy >= user.Hierarchy || hieararchy >= (await context.Guild.GetCurrentUserAsync().ConfigureAwait(false)).Hierarchy)
                return PreconditionResult.FromError(ErrorMessage);

            return PreconditionResult.FromSuccess();
        }

        /// <summary>
        ///     Determines the hierarchy of a target within a guild context based on its type.
        /// </summary>
        /// <remarks>
        ///     The order of the <paramref name="value"/> type checking in the <see langword="switch"/>
        ///     statement is crucial for determining the correct hierarchy value.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="value">The raw value of the target</param>
        /// <returns>
        ///     An integer representing the hierarchy of the target or <see cref="int.MaxValue"/> for roles that are either the
        ///     <see cref="IGuild.EveryoneRole"/> or the <see cref="IRole.IsManaged"/>, <see cref="int.MinValue"/> for <see cref="IUser"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when the parameter type is not supported by this precondition attribute.
        /// </exception>
        private static int GetTargetHieararchy(ICommandContext context, object value) => value switch
        {
            IRole role => (context.Guild.EveryoneRole.Id == role.Id || role.IsManaged) ? int.MaxValue : role.Position,
            IGuildUser guildUser => guildUser.Hierarchy,
            IUser => int.MinValue,
            _ => throw new ArgumentOutOfRangeException(nameof(value), "Attribute cannot be added to this parameter.")
        };
    }
}
