using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class PreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckPermissions(IMessage context);
    }
}
