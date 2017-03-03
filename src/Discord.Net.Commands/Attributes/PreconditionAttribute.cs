using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map);
    }
}
