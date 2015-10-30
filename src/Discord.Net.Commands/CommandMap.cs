using System;
using System.Collections.Generic;

namespace Discord.Commands
{
    internal class CommandMap
	{
		private CommandMap _parent;
		private Command _command;
		private readonly Dictionary<string, CommandMap> _subCommands;

		public CommandMap(CommandMap parent)
		{
			_parent = parent;
			_subCommands = new Dictionary<string, CommandMap>();
		}

		public CommandMap GetMap(string text)
		{
			CommandMap map;
			if (_subCommands.TryGetValue(text, out map))
				return map;
			else
				return null;
		}

		public Command GetCommand()
		{
			if (_command != null)
				return _command;
			else if (_parent != null)
				return _parent.GetCommand();
			else
				return null;
		}
		public Command GetCommand(string text)
		{
			return GetCommand(0, text.Split(' '));
		}
		public Command GetCommand(int index, string[] parts)
		{
			if (index != parts.Length)
			{
				string nextPart = parts[index];
				CommandMap nextGroup;
				if (_subCommands.TryGetValue(nextPart, out nextGroup))
				{
					var cmd = nextGroup.GetCommand(index + 1, parts);
					if (cmd != null)
						return cmd;
				}
			}

			if (_command != null)
				return _command;
			return null;
		}

		public void AddCommand(string text, Command command)
		{
			AddCommand(0, text.Split(' '), command);
		}
		public void AddCommand(int index, string[] parts, Command command)
		{
			if (index != parts.Length)
			{
				string nextPart = parts[index];
				CommandMap nextGroup;
				if (!_subCommands.TryGetValue(nextPart, out nextGroup))
				{
					nextGroup = new CommandMap(this);
					_subCommands.Add(nextPart, nextGroup);
				}
				nextGroup.AddCommand(index + 1, parts, command);
            }
			else
			{
				if (_command != null)
					throw new InvalidOperationException("A command has already been added with this path.");
				_command = command;
			}
		}
	}
}
