using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Commands
{
	public sealed class CommandParameter
	{
		public string Name { get; }
		public bool IsOptional { get; }
		public bool IsCatchAll { get; }

		public CommandParameter(string name, bool isOptional, bool isCatchAll)
		{
			Name = name;
			IsOptional = isOptional;
			IsCatchAll = isCatchAll;
		}
	}

	public sealed class Command
	{
		public string Text { get; }
		public int? MinArgs { get; private set; }
		public int? MaxArgs { get; private set; }
		public int MinPerms { get; internal set; }
        public bool IsHidden { get; internal set; }
        public string Description { get; internal set; }

		public IEnumerable<string> Aliases => _aliases;
		private string[] _aliases;

		public IEnumerable<CommandParameter> Parameters => _parameters;
		private CommandParameter[] _parameters;

		private Func<CommandEventArgs, Task> _handler;

		internal Command(string text)
		{
			Text = text;
            IsHidden = false;
			_aliases = new string[0];
			_parameters = new CommandParameter[0];
        }

		internal void SetAliases(string[] aliases)
		{
			_aliases = aliases;
		}

		internal void SetParameters(CommandParameter[] parameters)
		{
			_parameters = parameters;
			if (parameters != null)
			{
				if (parameters.Length == 0)
				{
					MinArgs = 0;
					MaxArgs = 0;
                }
				else
				{
					if (parameters[parameters.Length - 1].IsCatchAll)
						MaxArgs = null;
					else
						MaxArgs = parameters.Length;

					int? optionalStart = null;
					for (int i = parameters.Length - 1; i >= 0; i--)
					{
						if (parameters[i].IsOptional)
							optionalStart = i;
						else
							break;
					}
					if (optionalStart == null)
						MinArgs = MaxArgs;
					else
						MinArgs = optionalStart.Value;
				}
			}
		}

		internal void SetHandler(Func<CommandEventArgs, Task> func)
		{
			_handler = func;
		}
		internal void SetHandler(Action<CommandEventArgs> func)
		{
			_handler = e => { func(e); return TaskHelper.CompletedTask; };
		}
		
		internal Task Run(CommandEventArgs args)
		{
			var task = _handler(args);
			if (task != null)
				return task;
			else
				return TaskHelper.CompletedTask;
		}
	}
}
