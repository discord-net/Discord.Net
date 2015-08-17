using System;

namespace Discord
{
    public partial class DiscordBotClient : DiscordClient
	{
		public class CommandEventArgs
		{
			public readonly Message Message;
			public readonly Command Command;
			public readonly string[] Args;

			public User User => Message.User;
			public string UserId => Message.UserId;
			public Channel Channel => Message.Channel;
			public string ChannelId => Message.ChannelId;
			public Server Server => Message.Channel.Server;
			public string ServerId => Message.Channel.ServerId;

			public CommandEventArgs(Message message, Command command, string[] args)
			{
				Message = message;
				Command = command;
				Args = args;
			}
		}
		public class CommandErrorEventArgs : CommandEventArgs
		{
			public readonly Exception Exception;
			public CommandErrorEventArgs(Message message, Command command, string[] args, Exception ex)
				: base(message, command, args)
			{
				Exception = ex;
            }
		}

		public event EventHandler<CommandEventArgs> RanCommand;
		private void RaiseRanCommand(CommandEventArgs args)
		{
			if (RanCommand != null)
				RanCommand(this, args);
		}
		public event EventHandler<CommandErrorEventArgs> CommandError;
		private void RaiseCommandError(Message msg, Command command, string[] args, Exception ex)
		{
			if (CommandError != null)
				CommandError(this, new CommandErrorEventArgs(msg, command, args, ex));
		}
	}
}
