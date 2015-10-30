using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Commands
{
	/// <summary> A Discord.Net client with extensions for handling common bot operations like text commands. </summary>
	public partial class CommandsPlugin
    {
		private readonly DiscordClient _client;
		private readonly Func<User, int> _getPermissions;
        
		public IEnumerable<Command> Commands => _commands;
		private readonly List<Command> _commands;

		internal CommandMap Map => _map;
		private readonly CommandMap _map;

		public IEnumerable<char> CommandChars { get { return _commandChars; } set { _commandChars = value.ToArray(); } }
        private char[] _commandChars;
		
		public bool RequireCommandCharInPublic { get; set; }
		public bool RequireCommandCharInPrivate { get; set; }
        public bool HelpInPublic { get; set; }

        public CommandsPlugin(DiscordClient client, Func<User, int> getPermissions = null, bool builtInHelp = false)
        {
            _client = client;
            _getPermissions = getPermissions;
            
            _commands = new List<Command>();
			_map = new CommandMap(null);

			_commandChars = new char[] { '!' };
            RequireCommandCharInPublic = true;
            RequireCommandCharInPrivate = true;
            HelpInPublic = true;

            if (builtInHelp)
            {
                CreateCommand("help")
					.Parameter("command", ParameterType.Optional)
                    .Hide()
                    .Info("Returns information about commands.")
                    .Do(async e =>
                    {
                        if (e.Command.Text != "help")
                            await Reply(e, CommandDetails(e.Command));
                        else
                        {
                            if (e.Args == null)
							{
								int permissions = getPermissions(e.User);
								StringBuilder output = new StringBuilder();
                                output.AppendLine("These are the commands you can use:");
                                output.Append("`");
								output.Append(string.Join(", ", _commands.Select(x => permissions >= x.MinPerms && !x.IsHidden)));
                                output.Append("`");

                                if (_commandChars.Length == 1)
                                    output.AppendLine($"\nYou can use `{_commandChars[0]}` to call a command.");
                                else
                                    output.AppendLine($"\nYou can use `{string.Join(" ", CommandChars.Take(_commandChars.Length - 1))}` and `{_commandChars.Last()}` to call a command.");

                                output.AppendLine("`help <command>` can tell you more about how to use a command.");

                                await Reply(e, output.ToString());
                            }
                            else
                            {
								var cmd = _map.GetCommand(e.Args[0]);
                                if (cmd != null)
                                    await Reply(e, CommandDetails(cmd));
                                else
                                    await Reply(e, $"`{e.Args[0]}` is not a valid command.");
                            }
                        }
                    });

            }

            client.MessageReceived += async (s, e) =>
            {
                // This will need to be changed once a built in help command is made
                if (_commands.Count == 0)  return;
                if (e.Message.IsAuthor) return;

                string msg = e.Message.Text;
                if (msg.Length == 0) return;

                if (_commandChars.Length > 0)
                {
                    bool isPrivate = e.Message.Channel.IsPrivate;
                    bool hasCommandChar = CommandChars.Contains(msg[0]);
                    if (hasCommandChar)
                        msg = msg.Substring(1);

                    if (isPrivate && RequireCommandCharInPrivate && !hasCommandChar)
                        return; // If private, and command char is required, and it doesn't have it, ignore it.
                    if (!isPrivate && RequireCommandCharInPublic && !hasCommandChar)
                        return; // Same, but public.
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
					//Parse arguments
					CommandPart[] args;
					if (!CommandParser.ParseArgs(msg, argPos, command, out args))
					{
						CommandEventArgs errorArgs = new CommandEventArgs(e.Message, null, null, null);
						RaiseCommandError(CommandErrorType.InvalidInput, errorArgs);
						return;
					}
					int argCount = args.Length;

					//Get information for the rest of the steps
					int userPermissions = _getPermissions != null ? _getPermissions(e.Message.User) : 0;
					var eventArgs = new CommandEventArgs(e.Message, command, userPermissions, args.Select(x => x.Value).ToArray());

					// Check permissions
					if (userPermissions < command.MinPerms)
					{
						RaiseCommandError(CommandErrorType.BadPermissions, eventArgs);
						return;
					}

					//Check arg count
					if (argCount < command.MinArgs)
					{
						RaiseCommandError(CommandErrorType.BadArgCount, eventArgs);
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
        
        private string CommandDetails(Command command)
        {
            StringBuilder output = new StringBuilder();

            output.Append($"`{command.Text}`");

            if (command.MinArgs != null && command.MaxArgs != null)
            {
                if (command.MinArgs == command.MaxArgs)
                {
                    if (command.MaxArgs != 0)
                        output.Append($" {command.MinArgs.ToString()} Args");
                }
                else
                    output.Append($" {command.MinArgs.ToString()} - {command.MaxArgs.ToString()} Args");
            }
            else if (command.MinArgs != null && command.MaxArgs == null)
                output.Append($" ≥{command.MinArgs.ToString()} Args");
            else if (command.MinArgs == null && command.MaxArgs != null)
                output.Append($" ≤{command.MaxArgs.ToString()} Args");

            output.Append($": {command.Description ?? "No description set for this command."}");

            return output.ToString();
        }

        internal async Task Reply(CommandEventArgs e, string message)
        {
            if (HelpInPublic)
                await _client.SendMessage(e.Channel, message);
            else
                await _client.SendPrivateMessage(e.User, message);
        }

		public void CreateCommandGroup(string cmd, Action<CommandGroupBuilder> config = null)
			=> config(new CommandGroupBuilder(this, cmd, 0));
		public CommandBuilder CreateCommand(string cmd)
		{
			var command = new Command(cmd);
			_commands.Add(command);
			return new CommandBuilder(null, command, "");
		}

		internal void AddCommand(Command command)
		{
			_commands.Add(command);
			_map.AddCommand(command.Text, command);
		}
	}
}
