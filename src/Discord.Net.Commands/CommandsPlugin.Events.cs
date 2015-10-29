using System;

namespace Discord.Commands
{
	public class PermissionException : Exception { public PermissionException() : base("User does not have permission to run this command.") { } }
    public class ArgumentException : Exception { public ArgumentException() : base("This command requires more arguments.") { } }
	public class CommandEventArgs
	{
		public Message Message { get; }
		public Command Command { get; }
        public string MessageText { get; }
		public string CommandText { get; }
		public string ArgText { get; }
		public int? Permissions { get; }
		public string[] Args { get; }
		
		public User User => Message.User;
		public Channel Channel => Message.Channel;
		public Server Server => Message.Channel.Server;

		public CommandEventArgs(Message message, Command command, string messageText, string commandText, string argText, int? permissions, string[] args)
		{
			Message = message;
			Command = command;
            MessageText = messageText;
			CommandText = commandText;
			ArgText = argText;
			Permissions = permissions;
			Args = args;
		}
	}
	public class CommandErrorEventArgs : CommandEventArgs
	{
		public Exception Exception { get; }

		public CommandErrorEventArgs(CommandEventArgs baseArgs, Exception ex)
			: base(baseArgs.Message, baseArgs.Command, baseArgs.MessageText, baseArgs.CommandText, baseArgs.ArgText, baseArgs.Permissions, baseArgs.Args)
		{
			Exception = ex;
		}
	}
	public partial class CommandsPlugin
	{
		public event EventHandler<CommandEventArgs> RanCommand;
		private void RaiseRanCommand(CommandEventArgs args)
		{
			if (RanCommand != null)
				RanCommand(this, args);
		}
		public event EventHandler<CommandEventArgs> UnknownCommand;
		private void RaiseUnknownCommand(CommandEventArgs args)
		{
			if (UnknownCommand != null)
				UnknownCommand(this, args);
		}
		public event EventHandler<CommandErrorEventArgs> CommandError;
		private void RaiseCommandError(CommandEventArgs args, Exception ex)
		{
			if (CommandError != null)
				CommandError(this, new CommandErrorEventArgs(args, ex));
		}
	}
}
