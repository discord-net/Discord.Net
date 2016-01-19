using Discord.Commands.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    //TODO: Make this more friendly and expose it to be extendable
	public sealed class CommandBuilder
	{
		private readonly CommandService _service;
		private readonly Command _command;
		private readonly List<CommandParameter> _params;
		private readonly List<IPermissionChecker> _checks;
		private readonly List<string> _aliases;
		private readonly string _prefix;
		private bool _allowRequiredParams, _areParamsClosed;

		public CommandService Service => _service;

		internal CommandBuilder(CommandService service, string text, string prefix = "", string category = "", IEnumerable<IPermissionChecker> initialChecks = null)
		{
            _service = service;
            _prefix = prefix;

            _command = new Command(AppendPrefix(prefix, text));
            _command.Category = category;

			if (initialChecks != null)
				_checks = new List<IPermissionChecker>(initialChecks);
			else
				_checks = new List<IPermissionChecker>();

            _params = new List<CommandParameter>();
			_aliases = new List<string>();

			_allowRequiredParams = true;
			_areParamsClosed = false;
        }
		
		public CommandBuilder Alias(params string[] aliases)
		{
			_aliases.AddRange(aliases);
			return this;
		}
		/*public CommandBuilder Category(string category)
		{
			_command.Category = category;
			return this;
        }*/
		public CommandBuilder Description(string description)
		{
			_command.Description = description;
			return this;
		}
		public CommandBuilder Parameter(string name, ParameterType type = ParameterType.Required)
		{
			if (_areParamsClosed)
				throw new Exception($"No parameters may be added after a {nameof(ParameterType.Multiple)} or {nameof(ParameterType.Unparsed)} parameter.");
			if (!_allowRequiredParams && type == ParameterType.Required)
				throw new Exception($"{nameof(ParameterType.Required)} parameters may not be added after an optional one");

			_params.Add(new CommandParameter(name, type));

			if (type == ParameterType.Optional)
				_allowRequiredParams = false;
            if (type == ParameterType.Multiple || type == ParameterType.Unparsed)
				_areParamsClosed = true;
			return this;
		}
		public CommandBuilder Hide()
		{
			_command.IsHidden = true;
			return this;
		}
		public CommandBuilder AddCheck(IPermissionChecker check)
		{
			_checks.Add(check);
			return this;
		}
		public CommandBuilder AddCheck(Func<Command, User, Channel, bool> checkFunc, string errorMsg = null)
		{
			_checks.Add(new GenericPermissionChecker(checkFunc, errorMsg));
			return this;
		}

		public void Do(Func<CommandEventArgs, Task> func)
		{
			_command.SetRunFunc(func);
			Build();
		}
		public void Do(Action<CommandEventArgs> func)
		{
			_command.SetRunFunc(func);
			Build();
		}
		private void Build()
		{
			_command.SetParameters(_params.ToArray());
			_command.SetChecks(_checks.ToArray());
			_command.SetAliases(_aliases.Select(x => AppendPrefix(_prefix, x)).ToArray());
			_service.AddCommand(_command);
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
				return prefix;
		}
	}
	public class CommandGroupBuilder
	{
		private readonly CommandService _service;
		private readonly string _prefix;
		private readonly List<IPermissionChecker> _checks;
		private string _category;

		public CommandService Service => _service;

		internal CommandGroupBuilder(CommandService service, string prefix = "", string category = null, IEnumerable<IPermissionChecker> initialChecks = null)
		{
			_service = service;
			_prefix = prefix;
            _category = category;
			if (initialChecks != null)
				_checks = new List<IPermissionChecker>(initialChecks);
			else
				_checks = new List<IPermissionChecker>();
		}

		public CommandGroupBuilder Category(string category)
		{
			_category = category;
			return this;
		}
		public void AddCheck(IPermissionChecker checker)
		{
			_checks.Add(checker);
		}
		public void AddCheck(Func<Command, User, Channel, bool> checkFunc, string errorMsg = null)
		{
			_checks.Add(new GenericPermissionChecker(checkFunc, errorMsg));
		}

		public CommandGroupBuilder CreateGroup(string cmd, Action<CommandGroupBuilder> config)
		{
            config(new CommandGroupBuilder(_service, CommandBuilder.AppendPrefix(_prefix, cmd), _category, _checks));
			return this;
		}
		public CommandBuilder CreateCommand()
			=> CreateCommand("");
        public CommandBuilder CreateCommand(string cmd)
            => new CommandBuilder(_service, cmd, _prefix, _category, _checks);
	}
}
