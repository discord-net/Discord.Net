using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        /// <summary>
        /// Specify a group that this precondition belongs to. Preconditions of the same group require only one
        /// of the preconditions to pass in order to be successful (A || B). Specifying <see cref="Group"/> = <see langword="null"/>
        /// or not at all will require *all* preconditions to pass, just like normal (A &amp;&amp; B).
        /// </summary>
        public string Group { get; set; } = null;

        /// <summary>
        /// When overridden in a derived class, uses the supplied string
        /// as the error message if the precondition doesn't pass.
        /// Setting this for a class that doesn't override
        /// this property is a no-op.
        /// </summary>
        public virtual string ErrorMessage { get { return null; } set { } }

        public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services);
    }
}
