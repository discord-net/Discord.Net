using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Defines the type of command context (i.e. where the command is being executed).
    /// </summary>
    [Flags]
    public enum ContextType
    {
        /// <summary>
        ///     Specifies the command to be executed within a guild.
        /// </summary>
        Guild = 0x01,
        /// <summary>
        ///     Specifies the command to be executed within a DM.
        /// </summary>
        DM = 0x02,
        /// <summary>
        ///     Specifies the command to be executed within a group.
        /// </summary>
        Group = 0x04
    }

    /// <summary>
    ///     Requires the command to be invoked in a specified context (e.g. in guild, DM).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireContextAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     Gets the context required to execute the command.
        /// </summary>
        public ContextType Contexts { get; }

        /// <summary> Requires the command to be invoked in the specified context. </summary>
        /// <param name="contexts">The type of context the command can be invoked in. Multiple contexts can be specified by ORing the contexts together.</param>
        /// <example>
        /// <code language="cs">
        ///     [Command("secret")]
        ///     [RequireContext(ContextType.DM | ContextType.Group)]
        ///     public Task PrivateOnlyAsync()
        ///     {
        ///         return ReplyAsync("shh, this command is a secret");
        ///     }
        /// </code>
        /// </example>
        public RequireContextAttribute(ContextType contexts)
        {
            Contexts = contexts;
        }

        /// <inheritdoc />
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            bool isValid = false;

            if ((Contexts & ContextType.Guild) != 0)
                isValid = context.Channel is IGuildChannel;
            if ((Contexts & ContextType.DM) != 0)
                isValid = isValid || context.Channel is IDMChannel;
            if ((Contexts & ContextType.Group) != 0)
                isValid = isValid || context.Channel is IGroupChannel;

            if (isValid)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError($"Invalid context for command; accepted contexts: {Contexts}."));
        }
    }
}
