using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
	public sealed class Command
	{
		public readonly string[] Text;
		public readonly int MinArgs, MaxArgs;
		public readonly bool UseWhitelist;
		internal readonly Func<DiscordBotClient.CommandEventArgs, Task> Handler;

		public Command(string[] text, int minArgs, int maxArgs, bool useWhitelist, Func<DiscordBotClient.CommandEventArgs, Task> handler)
		{
			Text = text;
			MinArgs = minArgs;
			MaxArgs = maxArgs;
			UseWhitelist = useWhitelist;
			Handler = handler;
		}
	}

	/// <summary>
	/// A Discord.Net client with extensions for handling common bot operations like text commands.
	/// </summary>
	public partial class DiscordBotClient : DiscordClient
    {
		private List<Command> _commands;
		private List<string> _whitelist;

		public IEnumerable<Command> Commands => _commands;

		public char CommandChar { get; set; }
		public bool UseCommandChar { get; set; }
		public bool RequireCommandCharInPublic { get; set; }
		public bool RequireCommandCharInPrivate { get; set; }
		public bool AlwaysUseWhitelist { get; set; }

		public DiscordBotClient()
		{
			_commands = new List<Command>();
			_whitelist = new List<string>();

			CommandChar = '~';
			RequireCommandCharInPublic = true;
			RequireCommandCharInPrivate = true;
			AlwaysUseWhitelist = false;

			MessageCreated += async (s, e) =>
			{
				//Ignore messages from ourselves
				if (e.Message.UserId == UserId)
					return;

				//Check the global whitelist
				if (AlwaysUseWhitelist && !_whitelist.Contains(e.Message.UserId))
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
					if (args.Length < cmd.Text.Length)
						continue;

                    bool isValid = true;
					for (int j = 0; j < cmd.Text.Length; j++)
					{
						if (!string.Equals(args[j], cmd.Text[j], StringComparison.OrdinalIgnoreCase))
						{
							isValid = false;
							break;
						}
					}
					if (!isValid)
						continue;

					//Check Whitelist
					if (cmd.UseWhitelist && !_whitelist.Contains(e.Message.UserId))
						continue;

					//Check Arg Count
					int argCount = args.Length - cmd.Text.Length;
					if (argCount < cmd.MinArgs || argCount > cmd.MaxArgs)
						continue;

					//Run Command
                    string[] newArgs = new string[argCount];
					for (int j = 0; j < newArgs.Length; j++)
						newArgs[j] = args[j + cmd.Text.Length];
					
					var eventArgs = new CommandEventArgs(e.Message, cmd, newArgs);
                    RaiseRanCommand(eventArgs);
					try
					{
						var task = cmd.Handler(eventArgs);
						if (task != null)
							await task;
					}
					catch (Exception ex)
					{
						RaiseCommandError(e.Message, cmd, newArgs, ex);
					}
					break;
				}
			};
		}

		public void AddCommandGroup(string cmd, Action<CommandBuilder> config, bool useWhitelist = false)
		{
			config(new CommandBuilder(this, cmd, useWhitelist));
		}
		public void AddCommand(string cmd, int minArgs, int maxArgs, Action<CommandEventArgs> handler, bool useWhitelist = false)
		{
			AddCommand(cmd, minArgs, maxArgs, e => { handler(e); return null; }, useWhitelist);
		}
		public void AddCommand(string cmd, int minArgs, int maxArgs, Func<CommandEventArgs, Task> handler, bool useWhitelist = false)
		{
			_commands.Add(new Command(cmd.Split(' '), minArgs, maxArgs, useWhitelist, handler));
        }

		public void AddWhitelist(User user)
			=> AddWhitelist(user.Id);
		public void AddWhitelist(string userId)
		{
			_whitelist.Add(userId);
		}
    }
}
