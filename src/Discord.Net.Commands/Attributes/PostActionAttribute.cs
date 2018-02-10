using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PostActionAttribute : Attribute
    {
        public abstract Task ExecuteAsync(ICommandContext context, CommandInfo command, IResult result, IServiceProvider services);
    }
}
