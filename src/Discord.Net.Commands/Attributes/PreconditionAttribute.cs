using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        public abstract Task<PreconditionResult> CheckPermissions(IMessage context, Command executingCommand, object moduleInstance);
    }
}
