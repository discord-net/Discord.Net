using Discord.Commands;
using System;
using System.Collections.Generic;

namespace Discord
{
	/// <summary> A Discord.Net client with extensions for handling common bot operations like text commands. </summary>
	public partial class DiscordBotClient : DiscordClient
    {
        internal List<Command> _commands;

		public IEnumerable<Command> Commands => _commands;

		public char CommandChar { get; set; }
		public bool UseCommandChar { get; set; }
		public bool RequireCommandCharInPublic { get; set; }
		public bool RequireCommandCharInPrivate { get; set; }

		public DiscordBotClient(DiscordClientConfig config = null, Func<User, Server, int> getPermissions = null)
			: base(config)
		{
			_commands = new List<Command>();

			CommandChar = '~';
			UseCommandChar = true;
			RequireCommandCharInPublic = true;
			RequireCommandCharInPrivate = true;

			MessageCreated += async (s, e) =>
			{
				//If commands aren't being used, don't bother processing them
				if (_commands.Count == 0)
					return;

				//Ignore messages from ourselves
				if (e.Message.UserId == CurrentUserId)
					return;

				//Check for the command character
				string msg = e.Message.Text;
				if (UseCommandChar)
				{
					if (msg.Length == 0)
						return;
					bool isPrivate = e.Message.Channel.IsPrivate;
					bool hasCommandChar = msg[0] == CommandChar;
					if (hasCommandChar)
						msg = msg.Substring(1);
					if (!isPrivate && RequireCommandCharInPublic && !hasCommandChar)
						return;
					if (isPrivate && RequireCommandCharInPrivate && !hasCommandChar)
						return;
				}

				string[] args;
				if (!CommandParser.ParseArgs(msg, out args))
					return;

				for (int i = 0; i < _commands.Count; i++)
				{
					Command cmd = _commands[i];

					//Check Command Parts
					if (args.Length < cmd.Parts.Length)
						continue;

                    bool isValid = true;
					for (int j = 0; j < cmd.Parts.Length; j++)
					{
						if (!string.Equals(args[j], cmd.Parts[j], StringComparison.OrdinalIgnoreCase))
						{
							isValid = false;
							break;
						}
					}
					if (!isValid)
						continue;

					//Check Arg Count
					int argCount = args.Length - cmd.Parts.Length;
					if (argCount < cmd.MinArgs || argCount > cmd.MaxArgs)
						continue;

					//Clean Args
					string[] newArgs = new string[argCount];
					for (int j = 0; j < newArgs.Length; j++)
						newArgs[j] = args[j + cmd.Parts.Length];
					
					//Check Permissions
                    int permissions = getPermissions != null ? getPermissions(e.Message.User, e.Message.Channel?.Server) : 0;
					var eventArgs = new CommandEventArgs(e.Message, cmd, msg, permissions, newArgs);
					if (permissions < cmd.MinPerms)
					{
						RaiseCommandError(eventArgs, new PermissionException());
						return;
					}

					//Run Command					
                    RaiseRanCommand(eventArgs);
					try
					{
						var task = cmd.Handler(eventArgs);
						if (task != null)
							await task;
					}
					catch (Exception ex)
					{
						RaiseCommandError(eventArgs, ex);
					}
					break;
				}
			};
		}

		public void CreateCommandGroup(string cmd, Action<CommandGroupBuilder> config = null)
			=> config(new CommandGroupBuilder(this, cmd, 0));
		public CommandBuilder CreateCommand(string cmd)
		{
			var command = new Command(cmd);
			_commands.Add(command);
			return new CommandBuilder(command);
		}
    }
}
