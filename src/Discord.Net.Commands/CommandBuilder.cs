using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
	public sealed class CommandBuilder
	{
		private readonly CommandService _service;
		private readonly Command _command;
		private List<CommandParameter> _params;
		private bool _allowRequired, _isClosed;
		private string _prefix;

		internal CommandBuilder(CommandService service, Command command, string prefix, string category)
		{
			_service = service;
			_command = command;
			_command.Category = category;
			_params = new List<CommandParameter>();
			_prefix = prefix;
			_allowRequired = true;
			_isClosed = false;
        }

		public CommandBuilder Alias(params string[] aliases)
		{
			aliases = aliases.Select(x => AppendPrefix(_prefix, x)).ToArray();
            _command.SetAliases(aliases);
			return this;
		}
		/*public CommandBuilder Category(string category)
		{
			_command.Category = category;
			return this;
        }*/
		public CommandBuilder Info(string description)
		{
			_command.Description = description;
			return this;
		}
		public CommandBuilder Parameter(string name, ParameterType type = ParameterType.Required)
		{
			if (_isClosed)
				throw new Exception($"No parameters may be added after a {nameof(ParameterType.Multiple)} or {nameof(ParameterType.Unparsed)} parameter.");
			if (!_allowRequired && type == ParameterType.Required)
				throw new Exception($"{nameof(ParameterType.Required)} parameters may not be added after an optional one");

			_params.Add(new CommandParameter(name, type));

			if (type == ParameterType.Optional)
				_allowRequired = false;
            if (type == ParameterType.Multiple || type == ParameterType.Unparsed)
				_isClosed = true;
			return this;
		}
		public CommandBuilder Hide()
		{
			_command.IsHidden = true;
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
            _service.AddCommand(_command);
            foreach (var alias in _command.Aliases)
				_service.Map.AddCommand(alias, _command);
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
		internal readonly CommandService _service;
		private readonly string _prefix;
		private string _category;

		internal CommandGroupBuilder(CommandService service, string prefix)
		{
			_service = service;
			_prefix = prefix;
		}

		public CommandGroupBuilder Category(string category)
		{
			_category = category;
			return this;
		}

		public CommandGroupBuilder CreateGroup(string cmd, Action<CommandGroupBuilder> config = null)
		{
			config(new CommandGroupBuilder(_service, _prefix + ' ' + cmd));
			return this;
		}
		public CommandBuilder CreateCommand()
			=> CreateCommand("");
        public CommandBuilder CreateCommand(string cmd)
		{
            var command = new Command(CommandBuilder.AppendPrefix(_prefix, cmd));
            return new CommandBuilder(_service, command, _prefix, _category);
		}
	}
}
