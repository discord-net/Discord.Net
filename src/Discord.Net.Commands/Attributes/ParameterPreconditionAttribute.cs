using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class ParameterPreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckPermissions(ICommandContext context, ParameterInfo parameter, object value, IDependencyMap map);
    }
}