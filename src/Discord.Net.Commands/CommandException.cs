using System;

namespace Discord.Commands
{
    public class CommandException : Exception
    {
        public CommandInfo Command { get; }
        public OverloadInfo Overload { get; }
        public ICommandContext Context { get; }

        public CommandException(OverloadInfo overload, ICommandContext context, Exception ex)
            : base($"Error occurred executing {overload.GetLogText(context)}.", ex)
        {
            Overload = overload;
            Command = overload.Command;
            Context = context;
        }
    }
}
