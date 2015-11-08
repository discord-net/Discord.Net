using System;

namespace Discord.Commands
{
	public class CommandEventArgs
	{
		public Message Message { get; }
		public Command Command { get; }
		public string[] Args { get; }
		
		public User User => Message.User;
		public Channel Channel => Message.Channel;
		public Server Server => Message.Channel.Server;

		public CommandEventArgs(Message message, Command command, string[] args)
		{
			Message = message;
			Command = command;
			Args = args;
		}
	}

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

	public partial class CommandService
	{
		public event EventHandler<CommandEventArgs> RanCommand;
		private void RaiseRanCommand(CommandEventArgs args)
		{
			if (RanCommand != null)
				RanCommand(this, args);
		}
		public event EventHandler<CommandErrorEventArgs> CommandError;
		private void RaiseCommandError(CommandErrorType errorType, CommandEventArgs args, Exception ex = null)
		{
			if (CommandError != null)
				CommandError(this, new CommandErrorEventArgs(errorType, args, ex));
		}
	}
}
