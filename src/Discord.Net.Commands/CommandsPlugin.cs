using System;
using System.Collections.Generic;

namespace Discord.Commands
{
	/// <summary> A Discord.Net client with extensions for handling common bot operations like text commands. </summary>
	public partial class CommandsPlugin
    {
		private readonly DiscordClient _client;
		private List<Command> _commands;
		private Func<User, int> _getPermissions;

		public IEnumerable<Command> Commands => _commands;

		public char CommandChar { get; set; }
		public bool UseCommandChar { get; set; }
		public bool RequireCommandCharInPublic { get; set; }
		public bool RequireCommandCharInPrivate { get; set; }

		public CommandsPlugin(DiscordClient client, Func<User, int> getPermissions = null)
		{
			_client = client;
			_getPermissions = getPermissions;
			_commands = new List<Command>();

			CommandChar = '/';
			UseCommandChar = false;
			RequireCommandCharInPublic = true;
			RequireCommandCharInPrivate = true;

			client.MessageReceived += async (s, e) =>
			{
				//If commands aren't being used, don't bother processing them
				if (_commands.Count == 0)
					return;

				//Ignore messages from ourselves
				if (e.Message.User == client.CurrentUser)
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

				CommandPart[] args;
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
						if (!string.Equals(args[j].Value, cmd.Parts[j], StringComparison.OrdinalIgnoreCase))
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
						newArgs[j] = args[j + cmd.Parts.Length].Value;

					//Get ArgText
					string argText;
					if (argCount == 0)
						argText = "";
					else
						argText = msg.Substring(args[cmd.Parts.Length].Index);

					//Check Permissions
					int permissions = _getPermissions != null ? _getPermissions(e.Message.User) : 0;
					var eventArgs = new CommandEventArgs(e.Message, cmd, msg, argText, permissions, newArgs);
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
							await task.ConfigureAwait(false);
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

		internal void AddCommand(Command command)
		{
			_commands.Add(command);
		}
	}
}
