using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class ParameterPreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services);
    }
}