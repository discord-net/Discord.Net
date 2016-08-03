using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public abstract class PreconditionAttribute : Attribute
    {
        public abstract void CheckPermissions(PreconditionContext context);
    }
}
