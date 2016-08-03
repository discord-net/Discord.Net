using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class PermissionsContext
    {
        public Command ExecutingCommand { get; internal set; }
        public IMessage Message { get; internal set; }

        public bool Handled { get; set; }

        internal PermissionsContext(Command command, IMessage message)
        {
            ExecutingCommand = command;
            Message = message;

            Handled = false;
        }
    }
}
