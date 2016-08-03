using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class RequireGuildAttribute : PreconditionAttribute
    {
        public override void CheckPermissions(PreconditionContext context)
        {
            if (!(context.Message.Channel is IGuildChannel))
                context.Handled = true;
        }
    }
}
