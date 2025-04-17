using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Ensures that command parameters are passed within a correct hierarchical context.
    /// </summary>
    /// <remarks>
    ///     Useful for performing hierarchical operations within a guild, such as managing roles or users.
    ///     <note type="warning">
    ///         This supports <see cref="IRole"/>, <see cref="IGuildUser"/>, and <see cref="IUser"/> parameter types.
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
        public string ErrorMessage { get; set; } = "You cannot target anyone who is higher or equal in the hierarchy to you or the bot.";

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown when the parameter type is not supported by this precondition attribute.
        /// </exception>
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameterInfo, object value, IServiceProvider services)
        {
            if (context.User is not IGuildUser guildUser)
                return PreconditionResult.FromError(NotAGuildErrorMessage);

            var hieararchy = PermissionUtils.GetHieararchy(value);
            if (hieararchy >= guildUser.Hierarchy ||
                hieararchy >= (await context.Guild.GetCurrentUserAsync().ConfigureAwait(false)).Hierarchy)
                return PreconditionResult.FromError(ErrorMessage);

            return PreconditionResult.FromSuccess();
        }
    }
}
