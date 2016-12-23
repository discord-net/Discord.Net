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
    /// Require that the command be invoked in a specified context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireContextAttribute : PreconditionAttribute
    {
        public ContextType Contexts { get; }

        /// <summary>
        /// Require that the command be invoked in a specified context.
        /// </summary>
        /// <param name="contexts">The type of context the command can be invoked in. Multiple contexts can be specified by ORing the contexts together.</param>
        /// <example>
        /// <code language="c#">
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

        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            bool isValid = false;

            if ((Contexts & ContextType.Guild) != 0)
                isValid = isValid || context.Channel is IGuildChannel;
            if ((Contexts & ContextType.DM) != 0)
                isValid = isValid || context.Channel is IDMChannel;
            if ((Contexts & ContextType.Group) != 0)
                isValid = isValid || context.Channel is IGroupChannel;

            if (isValid)
                return Task.FromResult(PreconditionResult.FromSuccess());
            else
                return Task.FromResult(PreconditionResult.FromError($"Invalid context for command; accepted contexts: {Contexts}"));
        }
    }
}
