using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
	public sealed class CommandBuilder
	{
		private readonly CommandsPlugin _plugin;
		private readonly Command _command;
		private List<CommandParameter> _params;
		private bool _hasOptional, _hasCatchAll;
		private string _prefix;

		public CommandBuilder(CommandsPlugin plugin, Command command, string prefix)
		{
			_plugin = plugin;
			_command = command;
			_params = new List<CommandParameter>();
			_prefix = prefix;
        }

		public CommandBuilder Alias(params string[] aliases)
		{
			aliases = aliases.Select(x => AppendPrefix(_prefix, x)).ToArray();
            _command.SetAliases(aliases);
			return this;
		}
		public CommandBuilder Info(string description)
		{
			_command.Description = description;
			return this;
		}
		public CommandBuilder Parameter(string name, bool isOptional = false, bool isCatchAll = false)
		{
			if (_hasCatchAll)
				throw new Exception("No parameters may be added after the catch-all");
			if (_hasOptional && isOptional)
				throw new Exception("Non-optional parameters may not be added after an optional one");

			_params.Add(new CommandParameter(name, isOptional, isCatchAll));

			if (isOptional)
				_hasOptional = true;
            if (isCatchAll)
				_hasCatchAll = true;
			return this;
		}
		public CommandBuilder IsHidden()
		{
			_command.IsHidden = true;
			return this;
		}

		public CommandBuilder MinPermissions(int level)
		{
			_command.MinPerms = level;
            return this;
		}

		public void Do(Func<CommandEventArgs, Task> func)
		{
			_command.SetHandler(func);
			Build();
		}
		public void Do(Action<CommandEventArgs> func)
		{
			_command.SetHandler(func);
			Build();
		}
		private void Build()
		{
			_command.SetParameters(_params.ToArray());
			foreach (var alias in _command.Aliases)
				_plugin.Map.AddCommand(alias, _command);
			_plugin.AddCommand(_command);
		}

		internal static string AppendPrefix(string prefix, string cmd)
		{
			if (cmd != "")
			{
				if (prefix != "")
					return prefix + ' ' + cmd;
				else
					return cmd;
			}
			else
			{
				if (prefix != "")
					return prefix;
				else
					throw new ArgumentOutOfRangeException(nameof(cmd));
			}
		}
	}
	public sealed class CommandGroupBuilder
	{
		internal readonly CommandsPlugin _plugin;
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
            var command = new Command(CommandBuilder.AppendPrefix(_prefix, cmd));
			command.MinPerms = _defaultMinPermissions;
            return new CommandBuilder(_plugin, command, _prefix);
		}
	}
}
