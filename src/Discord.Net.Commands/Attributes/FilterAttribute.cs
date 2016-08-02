using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class FilterAttribute : Attribute
    {
        public abstract void OnCommandExecuting(CommandExecutionContext context);
    }
}
