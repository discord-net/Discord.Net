using System;
using System.Threading.Tasks;

namespace Discord.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class ParameterPreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckPermissions(CommandContext context, CommandParameter parameter, IDependencyMap map);
    }
}
