using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class RequireDMAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(IMessage context)
        {
            if (context.Channel is IGuildChannel)
                return Task.FromResult(PreconditionResult.FromError("Command must be used in a DM"));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
