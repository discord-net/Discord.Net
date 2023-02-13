using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Requires the parameter to pass the specified precondition before execution can begin.
    /// </summary>
    /// <seealso cref="PreconditionAttribute"/>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class ParameterPreconditionAttribute : Attribute
    {
        /// <summary>
        ///     Gets the error message to be returned if execution context doesn't pass the precondition check.
        /// </summary>
        /// <remarks>
        ///     When overridden in a derived class, uses the supplied string
        ///     as the error message if the precondition doesn't pass.
        ///     Setting this for a class that doesn't override
        ///     this property is a no-op.
        /// </remarks>
        public virtual string ErrorMessage { get; }

        /// <summary>
        ///     Checks whether the condition is met before execution of the command.
        /// </summary>
        /// <param name="context">The context of the command.</param>
        /// <param name="parameterInfo">The parameter of the command being checked against.</param>
        /// <param name="value">The raw value of the parameter.</param>
        /// <param name="services">The service collection used for dependency injection.</param>
        public abstract Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, IParameterInfo parameterInfo, object value,
            IServiceProvider services);
    }
}
