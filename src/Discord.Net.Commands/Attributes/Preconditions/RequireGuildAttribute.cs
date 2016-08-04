using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireGuildAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(IMessage context, Command executingCommand, object moduleInstance)
        {
            if (!(context.Channel is IGuildChannel))
                return Task.FromResult(PreconditionResult.FromError("Command must be used in a guild"));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
