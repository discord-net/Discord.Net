using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
	public sealed class CommandBuilder
	{
		private readonly Command _command;
		public CommandBuilder(Command command)
		{
			_command = command;
		}
		
		public CommandBuilder ArgsEqual(int argCount)
		{
			_command.MinArgs = argCount;
			_command.MaxArgs = argCount;
			return this;
		}
		public CommandBuilder ArgsAtLeast(int minArgCount)
		{
			_command.MinArgs = minArgCount;
			_command.MaxArgs = null;
			return this;
		}
		public CommandBuilder ArgsAtMost(int maxArgCount)
		{
			_command.MinArgs = null;
			_command.MaxArgs = maxArgCount;
			return this;
		}
		public CommandBuilder ArgsBetween(int minArgCount, int maxArgCount)
		{
			_command.MinArgs = minArgCount;
			_command.MaxArgs = maxArgCount;
			return this;
		}
		public CommandBuilder NoArgs()
		{
			_command.MinArgs = 0;
			_command.MaxArgs = 0;
			return this;
		}
		public CommandBuilder AnyArgs()
		{
			_command.MinArgs = null;
			_command.MaxArgs = null;
			return this;
		}

		public CommandBuilder MinPermissions(int level)
		{
			_command.MinPerms = level;
            return this;
		}

		public CommandBuilder Do(Func<CommandEventArgs, Task> func)
		{
			_command.Handler = func;
            return this;
		}
		public CommandBuilder Do(Action<CommandEventArgs> func)
		{
#if DNXCORE50
			_command.Handler = e => { func(e); return Task.CompletedTask; };
#else
			_command.Handler = e => { func(e); return Task.Delay(0); };
#endif
			return this;
		}
	}
	public sealed class CommandGroupBuilder
	{
		private readonly CommandsPlugin _plugin;
		private readonly string _prefix;
		private int _defaultMinPermissions;

		internal CommandGroupBuilder(CommandsPlugin plugin, string prefix, int defaultMinPermissions)
		{
			_plugin = plugin;
			_prefix = prefix;
			_defaultMinPermissions = defaultMinPermissions;
		}

		public void DefaultMinPermissions(int level)
		{
			_defaultMinPermissions = level;
		}

		public CommandGroupBuilder CreateCommandGroup(string cmd, Action<CommandGroupBuilder> config = null)
		{
			config(new CommandGroupBuilder(_plugin, _prefix + ' ' + cmd, _defaultMinPermissions));
			return this;
		}
		public CommandBuilder CreateCommand()
			=> CreateCommand("");
        public CommandBuilder CreateCommand(string cmd)
		{
			string text;
			if (cmd != "")
			{
				if (_prefix != "")
					text = _prefix + ' ' + cmd;
				else
					text = cmd;
			}
			else
			{
				if (_prefix != "")
					text = _prefix;
				else
					throw new ArgumentOutOfRangeException(nameof(cmd));
			}
            var command = new Command(text);
			command.MinPerms = _defaultMinPermissions;
			_plugin._commands.Add(command);
            return new CommandBuilder(command);
		}
	}
}
