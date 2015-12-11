using Discord.Commands.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Commands
{
	public enum ParameterType
	{
		/// <summary> Catches a single required parameter. </summary>
		Required,
		/// <summary> Catches a single optional parameter. </summary>
		Optional,
		/// <summary> Catches a zero or more optional parameters. </summary>
		Multiple,
		/// <summary> Catches all remaining text as a single optional parameter. </summary>
		Unparsed
	}
	public sealed class CommandParameter
	{
		public string Name { get; }
		public int Id { get; internal set; }
		public ParameterType Type { get; }

		public CommandParameter(string name, ParameterType type)
		{
			Name = name;
			Type = type;
		}
	}

	public sealed class Command
	{
		public string Text { get; }
		public string Category { get; internal set; }
        public bool IsHidden { get; internal set; }
        public string Description { get; internal set; }

		public IEnumerable<string> Aliases => _aliases;
		private string[] _aliases;

		public IEnumerable<CommandParameter> Parameters => _parameters;
		internal CommandParameter[] _parameters;
		
		private IPermissionChecker[] _checks;
		private Func<CommandEventArgs, Task> _runFunc;
		internal readonly Dictionary<string, CommandParameter> _parametersByName;

		internal Command(string text)
		{
			Text = text;
            IsHidden = false;
			_aliases = new string[0];
			_parameters = new CommandParameter[0];
			_parametersByName = new Dictionary<string, CommandParameter>();
        }

		public CommandParameter this[string name] => _parametersByName[name];

		internal void SetAliases(string[] aliases)
		{
			_aliases = aliases;
		}
		internal void SetParameters(CommandParameter[] parameters)
		{
			_parametersByName.Clear();
			for (int i = 0; i < parameters.Length; i++)
			{
				parameters[i].Id = i;
				_parametersByName[parameters[i].Name] = parameters[i];
            }
			_parameters = parameters;
        }
		internal void SetChecks(IPermissionChecker[] checks)
		{
			_checks = checks;
		}

		internal bool CanRun(User user, Channel channel, out string error)
		{
			for (int i = 0; i < _checks.Length; i++)
			{
				if (!_checks[i].CanRun(this, user, channel, out error))
                    return false;
			}
			error = null;
			return true;
		}

		internal void SetRunFunc(Func<CommandEventArgs, Task> func)
		{
			_runFunc = func;
		}
		internal void SetRunFunc(Action<CommandEventArgs> func)
		{
            _runFunc = TaskHelper.ToAsync(func);
		}
		internal Task Run(CommandEventArgs args)
		{
			var task = _runFunc(args);
			if (task != null)
				return task;
			else
				return TaskHelper.CompletedTask;
		}
	}
}
