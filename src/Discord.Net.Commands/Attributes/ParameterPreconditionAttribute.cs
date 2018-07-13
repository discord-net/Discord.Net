using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Requires the parameter to pass the specified precondition before execution can begin.
    /// </summary>
    /// <seealso cref="PreconditionAttribute"/>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class ParameterPreconditionAttribute : Attribute
    {
        /// <summary>
        ///     Checks whether the condition is met before execution of the command.
        /// </summary>
        /// <param name="context">The context of the command.</param>
        /// <param name="parameter">The parameter of the command being checked against.</param>
        /// <param name="value">The raw value of the parameter.</param>
        /// <param name="services">The service collection used for dependency injection.</param>
        public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services);
    }
}
