using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [Flags]
    public enum ContextType
    {
        Guild = 0x01,
        DM = 0x02,
        Group = 0x04
    }

    /// <summary>
    ///     Require that the command be invoked in a specified context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireContextAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     Require that the command be invoked in a specified context.
        /// </summary>
        /// <param name="contexts">
        ///     The type of context the command can be invoked in. Multiple contexts can be specified by ORing
        ///     the contexts together.
        /// </param>
        /// <example>
        ///     <code language="c#">
        ///     [Command("private_only")]
        ///     [RequireContext(ContextType.DM | ContextType.Group)]
        ///     public async Task PrivateOnly()
        ///     {
        ///     }
        /// </code>
        /// </example>
        public RequireContextAttribute(ContextType contexts)
        {
            Contexts = contexts;
        }

        public ContextType Contexts { get; }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command,
            IServiceProvider services)
        {
            var isValid = false;

            if ((Contexts & ContextType.Guild) != 0)
                isValid = context.Channel is IGuildChannel;
            if ((Contexts & ContextType.DM) != 0)
                isValid = isValid || context.Channel is IDMChannel;
            if ((Contexts & ContextType.Group) != 0)
                isValid = isValid || context.Channel is IGroupChannel;

            return Task.FromResult(isValid
                ? PreconditionResult.FromSuccess()
                : PreconditionResult.FromError($"Invalid context for command; accepted contexts: {Contexts}"));
        }
    }
}
