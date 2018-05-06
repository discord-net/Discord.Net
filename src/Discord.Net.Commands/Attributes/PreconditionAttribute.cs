using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Requires the module or class to pass the specified precondition before execution can begin.
    /// </summary>
    /// <seealso cref="ParameterPreconditionAttribute"/>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        /// <summary>
        ///     Specifies a group that this precondition belongs to.
        /// </summary>
        /// <remarks>
        ///     <see cref="Preconditions" /> of the same group require only one of the preconditions to pass in order to
        ///     be successful (A || B). Specifying <see cref="Group" /> = <c>null</c> or not at all will
        ///     require *all* preconditions to pass, just like normal (A &amp;&amp; B).
        /// </remarks>
        public string Group { get; set; } = null;

        /// <summary>
        ///     Checks if the <paramref name="command"/> has the sufficient permission to be executed.
        /// </summary>
        /// <param name="context">The context of the command.</param>
        /// <param name="command">The command being executed.</param>
        /// <param name="services">The service collection used for dependency injection.</param>
        public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services);
    }
}
