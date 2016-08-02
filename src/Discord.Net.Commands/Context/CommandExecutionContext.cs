using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    public class CommandExecutionContext
    {
        public Command ExecutingCommand { get; internal set; }
        public ParseResult ParseResult { get; internal set; }
        public IMessage Message { get; internal set; }

        public bool Handled { get; set; }

        internal CommandExecutionContext(Command command, ParseResult parseResult, IMessage message)
        {
            ExecutingCommand = command;
            ParseResult = parseResult;
            Message = message;

            Handled = false;
        }
    }
}
