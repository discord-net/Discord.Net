using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Commands
{
	/// <summary> A Discord.Net client with extensions for handling common bot operations like text commands. </summary>
	public partial class CommandService : IService
    {
		private DiscordClient _client;

		CommandServiceConfig Config { get; }
		public IEnumerable<Command> Commands => _commands;
		private readonly List<Command> _commands;

		internal CommandMap Map => _map;
		private readonly CommandMap _map;

		public CommandService(CommandServiceConfig config)
		{
			Config = config;
			_commands = new List<Command>();
			_map = new CommandMap(null);
		}

		void IService.Install(DiscordClient client)
		{
			_client = client;
			Config.Lock();

            if (Config.HelpMode != HelpMode.Disable)
            {
                CreateCommand("help")
					.Parameter("command", ParameterType.Multiple)
                    .Hide()
                    .Info("Returns information about commands.")
                    .Do(async e =>
                    {
						Channel channel = Config.HelpMode == HelpMode.Public ? e.Channel : await client.CreatePMChannel(e.User);
						if (e.Args.Length > 0) //Show command help
						{
							var cmd = _map.GetCommand(string.Join(" ", e.Args));
							if (cmd != null)
								await ShowHelp(cmd, e.User, channel);
							else
								await client.SendMessage(channel, "Unable to display help: unknown command.");
						}
                        else //Show general help
							await ShowHelp(e.User, channel);
                    });
            }

            client.MessageReceived += async (s, e) =>
            {
                if (_commands.Count == 0)  return;
                if (e.Message.IsAuthor) return;

                string msg = e.Message.Text;
                if (msg.Length == 0) return;

				//Check for command char if one is provided
				var chars = Config.CommandChars;
                if (chars.Length > 0)
                {
					if (!chars.Contains(msg[0]))
						return;
                    msg = msg.Substring(1);
                }

				//Parse command
				Command command;
				int argPos;
				CommandParser.ParseCommand(msg, _map, out command, out argPos);				
				if (command == null)
				{
					CommandEventArgs errorArgs = new CommandEventArgs(e.Message, null, null, null);
					RaiseCommandError(CommandErrorType.UnknownCommand, errorArgs);
					return;
				}
				else
				{
					int userPermissions = Config.PermissionResolver?.Invoke(e.Message.User) ?? 0;

					//Parse arguments
					string[] args;
					var error = CommandParser.ParseArgs(msg, argPos, command, out args);
                    if (error != null)
					{
						var errorArgs = new CommandEventArgs(e.Message, command, userPermissions, null);
						RaiseCommandError(error.Value, errorArgs);
						return;
					}
					
					var eventArgs = new CommandEventArgs(e.Message, command, userPermissions, args);

					// Check permissions
					if (userPermissions < command.MinPermissions)
					{
						RaiseCommandError(CommandErrorType.BadPermissions, eventArgs);
						return;
					}

					// Run the command
					try
					{
						RaiseRanCommand(eventArgs);
						await command.Run(eventArgs).ConfigureAwait(false);
					}
					catch (Exception ex)
					{
						RaiseCommandError(CommandErrorType.Exception, eventArgs, ex);
					}
				}
            };
        }

        public Task ShowHelp(User user, Channel channel)
        {
            int permissions = Config.PermissionResolver(user);

            StringBuilder output = new StringBuilder();
            output.AppendLine("These are the commands you can use:");

            output.Append(string.Join(", ", _map.SubCommands.Distinct()
                .Where(x => permissions >= x.MinPermissions && !x.IsHidden)
                .Select(x => '`' + x.Text + '`' +
                (x.Aliases.Count() > 0 ? ", `" + string.Join("`, `", x.Aliases) + '`' : ""))));

            var chars = Config.CommandChars;
            if (chars.Length > 0)
            {
                if (chars.Length == 1)
                    output.AppendLine($"\nYou can use `{chars[0]}` to call a command.");
                else
                    output.AppendLine($"\nYou can use `{string.Join(" ", chars.Take(chars.Length - 1))}` or `{chars.Last()}` to call a command.");
            }

            output.AppendLine("`help` `<command>` can tell you more about how to use a command.");

            return _client.SendMessage(channel, output.ToString());
        }

        public Task ShowHelp(Command command, User user, Channel channel)
        {
            StringBuilder output = new StringBuilder();
            output.Append($"`{command.Text}`");

            foreach (string s in command.Parameters.Where(x => x.Type == ParameterType.Required)
                .Select(x => x.Name))
                output.Append($" <`{s}`>");
            foreach (string s in command.Parameters.Where(x => x.Type == ParameterType.Optional)
                .Select(x => x.Name))
                output.Append($" [`{s}`]");

            if (command.Parameters.LastOrDefault(x => x.Type == ParameterType.Multiple) != null)
                output.Append(" [`...`]");

            if (command.Parameters.LastOrDefault(x => x.Type == ParameterType.Unparsed) != null)
                output.Append(" [`--`]");

            output.AppendLine($": {command.Description ?? "No description set for this command."}");

            var sub = _map.GetMap(command.Text).SubCommands;
            if (sub.Count() > 0)
            {
                int permissions = Config.PermissionResolver(user);
                output.AppendLine("Sub Commands: `" + string.Join("`, `", sub.Where(x => permissions >= x.MinPermissions && !x.IsHidden)
                    .Select(x => x.Text.Substring(command.Text.Length + 1))) + '`');
            }

            if (command.Aliases.Count() > 0)
                output.Append($"Aliases: `" + string.Join("`, `", command.Aliases) + '`');

            return _client.SendMessage(channel, output.ToString());
        }

        public void CreateCommandGroup(string cmd, Action<CommandGroupBuilder> config = null)
			=> config(new CommandGroupBuilder(this, cmd, 0));
		public CommandBuilder CreateCommand(string cmd)
		{
			var command = new Command(cmd);
			return new CommandBuilder(this, command, "");
		}

		internal void AddCommand(Command command)
		{
			_commands.Add(command);
			_map.AddCommand(command.Text, command);
		}
	}
}
