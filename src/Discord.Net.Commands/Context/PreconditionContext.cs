using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class PreconditionContext
    {
        public Command Command { get; internal set; }
        public IMessage Message { get; internal set; }

        public bool Handled { get; set; }

        internal PreconditionContext(Command command, IMessage message)
        {
            Command = command;
            Message = message;

            Handled = false;
        }
    }
}
