using System;

namespace Discord.Commands
{
    public enum CommandErrorType { Exception, UnknownCommand, BadPermissions, BadArgCount, InvalidInput }
    public class CommandErrorEventArgs : CommandEventArgs
    {
        public CommandErrorType ErrorType { get; }
        public Exception Exception { get; }

        public CommandErrorEventArgs(CommandErrorType errorType, CommandEventArgs baseArgs, Exception ex)
            : base(baseArgs.Message, baseArgs.Command, baseArgs.Args)
        {
            Exception = ex;
            ErrorType = errorType;
        }
    }
}
