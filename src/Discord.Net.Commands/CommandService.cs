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
		public CommandServiceConfig Config { get; }

		//AllCommands store a flattened collection of all commands
		public IEnumerable<Command> AllCommands => _allCommands;
		private readonly List<Command> _allCommands;

		//Command map stores all commands by their input text, used for fast resolving and parsing
		internal CommandMap Map => _map;
		private readonly CommandMap _map;

		//Groups store all commands by their module, used for more informative help
		internal IEnumerable<CommandMap> Categories => _categories.Values;
        private readonly Dictionary<string, CommandMap> _categories;


		public CommandService(CommandServiceConfig config)
		{
			Config = config;
			_allCommands = new List<Command>();
			_map = new CommandMap(null, null);
			_categories = new Dictionary<string, CommandMap>();
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
                if (_allCommands.Count == 0)  return;
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
					CommandEventArgs errorArgs = new CommandEventArgs(e.Message, null, null);
					RaiseCommandError(CommandErrorType.UnknownCommand, errorArgs);
					return;
				}
				else
				{
					//Parse arguments
					string[] args;
					var error = CommandParser.ParseArgs(msg, argPos, command, out args);
                    if (error != null)
					{
						var errorArgs = new CommandEventArgs(e.Message, command, null);
						RaiseCommandError(error.Value, errorArgs);
						return;
					}
					
					var eventArgs = new CommandEventArgs(e.Message, command, args);

					// Check permissions
					if (!command.CanRun(eventArgs.User, eventArgs.Channel))
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
            StringBuilder output = new StringBuilder();
			/*output.AppendLine("These are the commands you can use:");
			output.Append(string.Join(", ", _map.SubCommands
                .Where(x => x.CanRun(user, channel) && !x.IsHidden)
                .Select(x => '`' + x.Text + '`' +
                (x.Aliases.Count() > 0 ? ", `" + string.Join("`, `", x.Aliases) + '`' : ""))));
			output.AppendLine("\nThese are the groups you can access:");
			output.Append(string.Join(", ", _map.SubGroups
				.Where(x => /*x.CanRun(user, channel)*//* && !x.IsHidden)
				.Select(x => '`' + x.Text + '`')));*/

			bool isFirstCategory = true;
			foreach (var category in _categories)
			{
				bool isFirstItem = true;
				foreach (var item in category.Value.Items)
				{
					var map = item.Value;
					if (!map.IsHidden && map.CanRun(user, channel))
					{
						if (isFirstItem)
						{
							isFirstItem = false;
							//This is called for the first item in each category. If we never get here, we dont bother writing the header for a category type (since it's empty)
							if (isFirstCategory)
							{
								isFirstCategory = false;
								//Called for the first non-empty category
								output.AppendLine("These are the commands you can use:");
							}
							else
								output.AppendLine();
							if (category.Key != "")
							{
								output.Append(Format.Bold(category.Key));
								output.Append(": ");
							}
						}
						else
							output.Append(", ");
						output.Append('`');
						output.Append(map.Text);
						if (map.Items.Any())
							output.Append(@"\*");
						output.Append('`');
                    }
				}
			}

			if (output.Length == 0)
				output.Append("There are no commands you have permission to run.");
			else
			{
				output.AppendLine();

				var chars = Config.CommandChars;
				if (chars.Length > 0)
				{
					if (chars.Length == 1)
						output.AppendLine($"You can use `{chars[0]}` to call a command.");
					else
						output.AppendLine($"You can use `{string.Join(" ", chars.Take(chars.Length - 1))}` or `{chars.Last()}` to call a command.");
				}

				output.AppendLine("`help <command>` can tell you more about how to use a command.");
			}

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

            var sub = _map.GetItem(command.Text).SubCommands;
            if (sub.Count() > 0)
            {
                output.AppendLine("Sub Commands: `" + string.Join("`, `", sub.Where(x => x.CanRun(user, channel) && !x.IsHidden)
                    .Select(x => x.Text.Substring(command.Text.Length + 1))) + '`');
            }

            if (command.Aliases.Count() > 0)
                output.Append($"Aliases: `" + string.Join("`, `", command.Aliases) + '`');

            return _client.SendMessage(channel, output.ToString());
        }

		public void CreateGroup(string cmd, Action<CommandGroupBuilder> config = null)
		{
			var builder = new CommandGroupBuilder(this, cmd);
            if (config != null)
				config(builder);
		}
		public CommandBuilder CreateCommand(string cmd)
		{
			var command = new Command(cmd);
			return new CommandBuilder(this, command, "", "");
		}

		internal void AddCommand(Command command)
		{
			_allCommands.Add(command);
			CommandMap category;
            string categoryName = command.Category ?? "";
			if (!_categories.TryGetValue(categoryName, out category))
			{
				category = new CommandMap(null, "");
				_categories.Add(categoryName, category);
			}
			_map.AddCommand(command.Text, command);
		}
	}
}
