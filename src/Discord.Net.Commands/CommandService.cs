using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Commands
{
	public partial class CommandService : IService
    {
        private readonly List<Command> _allCommands;
        private readonly Dictionary<string, CommandMap> _categories;
        private readonly CommandMap _map; //Command map stores all commands by their input text, used for fast resolving and parsing
        
        public CommandServiceConfig Config { get; }
        public CommandGroupBuilder Root { get; }
        public DiscordClient Client { get; private set; }

		//AllCommands store a flattened collection of all commands
		public IEnumerable<Command> AllCommands => _allCommands;		
		//Groups store all commands by their module, used for more informative help
		internal IEnumerable<CommandMap> Categories => _categories.Values;

        public event EventHandler<CommandEventArgs> CommandExecuted = delegate { };
        public event EventHandler<CommandErrorEventArgs> CommandErrored = delegate { };

        private void OnCommand(CommandEventArgs args)
            => CommandExecuted(this, args);
        private void OnCommandError(CommandErrorType errorType, CommandEventArgs args, Exception ex = null)
            => CommandErrored(this, new CommandErrorEventArgs(errorType, args, ex));

        public CommandService()
            : this(new CommandServiceConfigBuilder())
        {
        }
        public CommandService(CommandServiceConfigBuilder builder)
            : this(builder.Build())
        {
            if (builder.ExecuteHandler != null)
                CommandExecuted += builder.ExecuteHandler;
            if (builder.ErrorHandler != null)
                CommandErrored += builder.ErrorHandler;
        }
        public CommandService(CommandServiceConfig config)
		{
            Config = config;

			_allCommands = new List<Command>();
			_map = new CommandMap();
			_categories = new Dictionary<string, CommandMap>();
            Root = new CommandGroupBuilder(this);
		}

		void IService.Install(DiscordClient client)
		{
            Client = client;

			if (Config.HelpMode != HelpMode.Disabled)
            {
				CreateCommand("help")
					.Parameter("command", ParameterType.Multiple)
                    .Hide()
                    .Description("Returns information about commands.")
                    .Do(async e =>
                    {
						Channel replyChannel = Config.HelpMode == HelpMode.Public ? e.Channel : await e.User.CreatePMChannel().ConfigureAwait(false);
						if (e.Args.Length > 0) //Show command help
						{
							var map = _map.GetItem(string.Join(" ", e.Args));
							if (map != null)
								await ShowCommandHelp(map, e.User, e.Channel, replyChannel).ConfigureAwait(false);
							else
								await replyChannel.SendMessage("Unable to display help: Unknown command.").ConfigureAwait(false);
						}
                        else //Show general help							
							await ShowGeneralHelp(e.User, e.Channel, replyChannel).ConfigureAwait(false);
                    });
            }

            client.MessageReceived += async (s, e) =>
            {
                if (_allCommands.Count == 0)  return;

                if (Config.IsSelfBot)
                {
                    if (e.Message.User == null || e.Message.User.Id != Client.CurrentUser.Id) return; // Will only listen to Self
                }
                else
                    if (e.Message.User == null || e.Message.User.Id == Client.CurrentUser.Id) return; // Normal expected behavior for bots

                string msg = e.Message.RawText;
                if (msg.Length == 0) return;

                string cmdMsg = null;

                //Check for command char
                if (Config.PrefixChar.HasValue)
                {
                    if (msg[0] == Config.PrefixChar.Value)
                        cmdMsg = msg.Substring(1);
                }

                //Check for mention
                if (cmdMsg == null && Config.AllowMentionPrefix)
                {
                    string mention = client.CurrentUser.Mention;
                    if (msg.StartsWith(mention) && msg.Length > mention.Length)
                        cmdMsg = msg.Substring(mention.Length + 1);
                    else
                    {
                        mention = $"@{client.CurrentUser.Name}";
                        if (msg.StartsWith(mention) && msg.Length > mention.Length)
                            cmdMsg = msg.Substring(mention.Length + 1);
                    }

                    string mention2 = client.CurrentUser.NicknameMention;
                    if (mention2 != null)
                    {
                        if (msg.StartsWith(mention2) && msg.Length > mention2.Length)
                            cmdMsg = msg.Substring(mention2.Length + 1);
                        else
                        {
                            mention2 = $"@{client.CurrentUser.Name}";
                            if (msg.StartsWith(mention2) && msg.Length > mention2.Length)
                                cmdMsg = msg.Substring(mention2.Length + 1);
                        }
                    }
                }
                
                //Check using custom activator
                if (cmdMsg == null && Config.CustomPrefixHandler != null)
                {
                    int index = Config.CustomPrefixHandler(e.Message);
                    if (index >= 0)
                        cmdMsg = msg.Substring(index);
                }
                
                if (cmdMsg == null) return;

                //Parse command
                IEnumerable<Command> commands;
				int argPos;
				CommandParser.ParseCommand(cmdMsg, _map, out commands, out argPos);				
				if (commands == null)
				{
					CommandEventArgs errorArgs = new CommandEventArgs(e.Message, null, null);
					OnCommandError(CommandErrorType.UnknownCommand, errorArgs);
					return;
				}
				else
				{
					foreach (var command in commands)
					{
						//Parse arguments
						string[] args;
						var error = CommandParser.ParseArgs(cmdMsg, argPos, command, out args);
						if (error != null)
						{
							if (error == CommandErrorType.BadArgCount)
								continue;
							else
							{
								var errorArgs = new CommandEventArgs(e.Message, command, null);
								OnCommandError(error.Value, errorArgs);
								return;
							}
						}

						var eventArgs = new CommandEventArgs(e.Message, command, args);

						// Check permissions
						string errorText;
						if (!command.CanRun(eventArgs.User, eventArgs.Channel, out errorText))
						{
							OnCommandError(CommandErrorType.BadPermissions, eventArgs, errorText != null ? new Exception(errorText) : null);
							return;
						}

						// Run the command
						try
						{
							OnCommand(eventArgs);
							await command.Run(eventArgs).ConfigureAwait(false);
						}
						catch (Exception ex)
						{
							OnCommandError(CommandErrorType.Exception, eventArgs, ex);
						}
						return;
					}
					var errorArgs2 = new CommandEventArgs(e.Message, null, null);
					OnCommandError(CommandErrorType.BadArgCount, errorArgs2);
				}
            };
        }

        public Task ShowGeneralHelp(User user, Channel channel, Channel replyChannel = null)
        {
            StringBuilder output = new StringBuilder();
			bool isFirstCategory = true;
			foreach (var category in _categories)
			{
				bool isFirstItem = true;
				foreach (var group in category.Value.SubGroups)
				{
					string error;
					if (group.IsVisible && (group.HasSubGroups || group.HasNonAliases) && group.CanRun(user, channel, out error))
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
						output.Append(group.Name);
						if (group.HasSubGroups)
							output.Append("*");
						output.Append('`');
                    }
				}
			}

			if (output.Length == 0)
				output.Append("There are no commands you have permission to run.");
			else
			{
				output.Append("\n\n");

                //TODO: Should prefix be stated in the help message or not?
                /*StringBuilder builder = new StringBuilder();
                if (Config.PrefixChar != null)
                {
                    builder.Append('`');
                    builder.Append(Config.PrefixChar.Value);
                    builder.Append('`');
                }
                if (Config.AllowMentionPrefix)
                {
                    if (builder.Length > 0)
                        builder.Append(" or ");
                    builder.Append(Client.CurrentUser.Mention);
                }
                if (builder.Length > 0)
                    output.AppendLine($"Start your message with {builder.ToString()} to run a command.");*/
                output.AppendLine($"Run `help <command>` for more information.");
            }

            return (replyChannel ?? channel).SendMessage(output.ToString());
        }

		private Task ShowCommandHelp(CommandMap map, User user, Channel channel, Channel replyChannel = null)
        {
			StringBuilder output = new StringBuilder();

			IEnumerable<Command> cmds = map.Commands;
			bool isFirstCmd = true;
			string error;
			if (cmds.Any())
			{
				foreach (var cmd in cmds)
				{
					if (!cmd.CanRun(user, channel, out error)) { }
						//output.AppendLine(error ?? DefaultPermissionError);
					else
					{
						if (isFirstCmd)
							isFirstCmd = false;
						else
							output.AppendLine();
						ShowCommandHelpInternal(cmd, user, channel, output);
					}
				}
			}
			else
			{
				output.Append('`');
				output.Append(map.FullName);
				output.Append("`\n");
			}

			bool isFirstSubCmd = true;
			foreach (var subCmd in map.SubGroups.Where(x => x.CanRun(user, channel, out error) && x.IsVisible))
			{
				if (isFirstSubCmd)
				{
					isFirstSubCmd = false;
					output.AppendLine("Sub Commands: ");
				}
				else
					output.Append(", ");
				output.Append('`');
				output.Append(subCmd.Name);
				if (subCmd.SubGroups.Any())
					output.Append("*");
				output.Append('`');
			}

			if (isFirstCmd && isFirstSubCmd) //Had no commands and no subcommands
			{
				output.Clear();
				output.AppendLine("There are no commands you have permission to run.");
			}

			return (replyChannel ?? channel).SendMessage(output.ToString());
		}
		public Task ShowCommandHelp(Command command, User user, Channel channel, Channel replyChannel = null)
		{
			StringBuilder output = new StringBuilder();
			string error;
			if (!command.CanRun(user, channel, out error))
				output.AppendLine(error ?? "You do not have permission to access this command.");
			else
				ShowCommandHelpInternal(command, user, channel, output);
            return (replyChannel ?? channel).SendMessage(output.ToString());
		}
		private void ShowCommandHelpInternal(Command command, User user, Channel channel, StringBuilder output)
		{
			output.Append('`');
			output.Append(command.Text);
			foreach (var param in command.Parameters)
			{
				switch (param.Type)
				{
					case ParameterType.Required:
						output.Append($" <{param.Name}>");
						break;
					case ParameterType.Optional:
						output.Append($" [{param.Name}]");
						break;
					case ParameterType.Multiple:
						output.Append($" [{param.Name}...]");
						break;
					case ParameterType.Unparsed:
						output.Append($" [-]");
						break;
				}
			}
			output.AppendLine("`");
			output.AppendLine($"{command.Description ?? "No description."}");

			if (command.Aliases.Any())
				output.AppendLine($"Aliases: `" + string.Join("`, `", command.Aliases) + '`');
        }

		public void CreateGroup(string cmd, Action<CommandGroupBuilder> config = null) => Root.CreateGroup(cmd, config);
		public CommandBuilder CreateCommand(string cmd) => Root.CreateCommand(cmd);

		internal void AddCommand(Command command)
		{
			_allCommands.Add(command);

			//Get category
			CommandMap category;
            string categoryName = command.Category ?? "";
			if (!_categories.TryGetValue(categoryName, out category))
			{
				category = new CommandMap();
				_categories.Add(categoryName, category);
			}

			//Add main command
			category.AddCommand(command.Text, command, false);
            _map.AddCommand(command.Text, command, false);

			//Add aliases
			foreach (var alias in command.Aliases)
			{
				category.AddCommand(alias, command, true);
				_map.AddCommand(alias, command, true);
			}
		}
	}
}
