using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        /// <summary>
        /// Specify a group that this precondition belongs to. Preconditions of the same group require only one
        /// of the preconditions to pass in order to be successful (A || B). Specifying <see cref="Group"/> = 0
        /// or not at all will require *all* preconditions to pass, just like normal (A && B).
        /// </summary>
        public int Group { get; set; } = 0;

        public abstract Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services);
    }
}
