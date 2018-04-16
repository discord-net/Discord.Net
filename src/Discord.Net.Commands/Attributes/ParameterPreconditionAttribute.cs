using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     Requires the parameter to pass the specified precondition before execution can begin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class ParameterPreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services);
    }
}
