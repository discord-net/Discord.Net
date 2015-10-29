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
		private Func<User, int> _getPermissions;

        private Dictionary<string, Command> _commands;
        
        public Dictionary<string, Command> Commands => _commands;

		public char CommandChar { get { return CommandChars[0]; } set { CommandChars = new List<char> { value }; } } // This could possibly be removed entirely. Not sure.
        public List<char> CommandChars { get; set; }
		public bool UseCommandChar { get; set; }
		public bool RequireCommandCharInPublic { get; set; }
		public bool RequireCommandCharInPrivate { get; set; }
        public bool HelpInPublic { get; set; }

        public CommandsPlugin(DiscordClient client, Func<User, int> getPermissions = null, bool builtInHelp = false)
        {
            _client = client; // Wait why is this even set
            _getPermissions = getPermissions;
            
            _commands = new Dictionary<string, Command>();

            CommandChar = '!'; // Kept around to keep from possibly throwing an error. Might not be necessary.
            CommandChars = new List<char> { '!', '?', '/' };
            UseCommandChar = true;
            RequireCommandCharInPublic = true;
            RequireCommandCharInPrivate = true;
            HelpInPublic = true;

            if (builtInHelp)
            {
                CreateCommand("help")
                    .ArgsBetween(0, 1)
                    .IsHidden()
                    .Desc("Returns information about commands.")
                    .Do(async e =>
                    {
                        if (e.Command.Text != "help")
                        {
                            await Reply(e, CommandDetails(e.Command));
                        }
                        else
                        {
                            if (e.Args == null)
                            {
                                StringBuilder output = new StringBuilder();
                                bool first = true;
                                output.AppendLine("These are the commands you can use:");
                                output.Append("`");
                                int permissions = getPermissions(e.User);
                                foreach (KeyValuePair<string, Command> k in _commands)
                                {
                                    if (permissions >= k.Value.MinPerms && !k.Value.IsHidden)
                                        if (first)
                                        {
                                            output.Append(k.Key);
                                            first = false;
                                        }
                                        else
                                            output.Append($", {k.Key}");
                                }
                                output.Append("`");

                                if (CommandChars.Count == 1)
                                    output.AppendLine($"{Environment.NewLine}You can use `{CommandChars[0]}` to call a command.");
                                else
                                    output.AppendLine($"{Environment.NewLine}You can use `{String.Join(" ", CommandChars.Take(CommandChars.Count - 1))}` and `{CommandChars.Last()}` to call a command.");

                                output.AppendLine("`help <command>` can tell you more about how to use a command.");

                                await Reply(e, output.ToString());
                            }
                            else
                            {
                                if (_commands.ContainsKey(e.Args[0]))
                                    await Reply(e, CommandDetails(_commands[e.Args[0]]));
                                else
                                    await Reply(e, $"`{e.Args[0]}` is not a valid command.");
                            }
                        }
                    });

            }

            client.MessageReceived += async (s, e) =>
            {
                // This will need to be changed once a built in help command is made
                if (_commands.Count == 0) 
                    return;

                if (e.Message.IsAuthor)
                    return;

                string msg = e.Message.Text;

                if (msg.Length == 0)
                    return;

                if (UseCommandChar)
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

                string cmd;
                CommandPart[] args;
                if (!CommandParser.Parse(msg, out cmd, out args))
                    return;

                if (_commands.ContainsKey(cmd))
                {
                    Command comm = _commands[cmd];
                    
                    //Get ArgText
                    int argCount = args.Length;
                    string argText;
                    if (argCount == 0)
                        argText = "";
                    else
                        argText = msg.Substring(args[0].Index);

                    //Clean Args
                    string[] newArgs = null;

                    if (comm.MaxArgs != null && argCount > 0)
                    {
                        newArgs = new string[(int)comm.MaxArgs];
                        for (int j = 0; j < newArgs.Length; j++)
                            newArgs[j] = args[j].Value;
                    }
                    else if (comm.MaxArgs == null && comm.MinArgs == null)
                    {
                        newArgs = new string[argCount];
                        for (int j = 0; j < newArgs.Length; j++)
                            newArgs[j] = args[j].Value;
                    }

                        // Check permissions here
                        int permissions = _getPermissions != null ? _getPermissions(e.Message.User) : 0;
                    var eventArgs = new CommandEventArgs(e.Message, comm, msg, cmd, argText, permissions, newArgs);
                    if (permissions < comm.MinPerms)
                    {
                        RaiseCommandError(eventArgs, new PermissionException());
                        return;
                    }
                    
                    //Check Arg Count
                    if (argCount < comm.MinArgs)
                    {
                        RaiseCommandError(eventArgs, new ArgumentException());
                        if (builtInHelp)
                            await _commands["help"].Handler(eventArgs);
                        return;
                    }

                    // Actually run the command here
                    RaiseRanCommand(eventArgs);
                    try
                    {
                        var task = comm.Handler(eventArgs);
                        if (task != null)
                            await task.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        RaiseCommandError(eventArgs, ex);
                    }
                }
                else
                {
                    CommandEventArgs eventArgs = new CommandEventArgs(e.Message, null, msg, cmd, null, null, null);
                    RaiseUnknownCommand(eventArgs);
                    if (builtInHelp)
                        await Reply(eventArgs, $"The command `{cmd}` does not exist.");
                    return;
                }
            };
        }
        
        internal string CommandDetails(Command comm)
        {
            StringBuilder output = new StringBuilder();

            output.Append($"`{comm.Text}`");

            if (comm.MinArgs != null && comm.MaxArgs != null)
            {
                if (comm.MinArgs == comm.MaxArgs)
                {
                    if (comm.MaxArgs != 0)
                        output.Append($" {comm.MinArgs.ToString()} Args");
                }
                else
                    output.Append($" {comm.MinArgs.ToString()} - {comm.MaxArgs.ToString()} Args");
            }
            else if (comm.MinArgs != null && comm.MaxArgs == null)
            {
                output.Append($" ≥{comm.MinArgs.ToString()} Args");
            }
            else if (comm.MinArgs == null && comm.MaxArgs != null)
            {
                output.Append($" ≤{comm.MaxArgs.ToString()} Args");
            }

            output.Append($": {comm.Description}");

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
			_commands.Add(cmd, command);
			return new CommandBuilder(command);
		}

		internal void AddCommand(Command command)
		{
			_commands.Add(command.Text, command);
		}
	}
}
