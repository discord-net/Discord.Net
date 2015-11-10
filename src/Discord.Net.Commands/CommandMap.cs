using System;
using System.Collections.Generic;

namespace Discord.Commands
{
	//Represents either a single function, command group, or both
	internal class CommandMap
	{
		private readonly CommandMap _parent;
		private readonly string _name, _fullName;

		private readonly List<Command> _commands;
		private readonly Dictionary<string, CommandMap> _items;
		private bool _isHidden;

		public string Name => _name;
		public string FullName => _fullName;
		public bool IsHidden => _isHidden;
		public IEnumerable<Command> Commands => _commands;
		public IEnumerable<CommandMap> SubGroups => _items.Values;
		/*public IEnumerable<Command> SubCommands => _items.Select(x => x.Value._command).Where(x => x != null);
		public IEnumerable<CommandMap> SubGroups => _items.Select(x => x.Value).Where(x => x._items.Count > 0);*/

		public CommandMap(CommandMap parent, string name, string fullName)
		{
			_parent = parent;
			_name = name;
			_fullName = fullName;
            _items = new Dictionary<string, CommandMap>();
			_commands = new List<Command>();
			_isHidden = true;
        }
		
		public CommandMap GetItem(string text)
		{
			return GetItem(0, text.Split(' '));
		}
		public CommandMap GetItem(int index, string[] parts)
		{
			if (index != parts.Length)
			{
				string nextPart = parts[index];
				CommandMap nextGroup;
				if (_items.TryGetValue(nextPart, out nextGroup))
					return nextGroup.GetItem(index + 1, parts);
				else
					return null;
			}
			return this;
		}

		public IEnumerable<Command> GetCommands()
		{
			if (_commands.Count > 0)
				return _commands;
			else if (_parent != null)
				return _parent.GetCommands();
			else
				return null;
		}
		public IEnumerable<Command> GetCommands(string text)
		{
			return GetCommands(0, text.Split(' '));
		}
		public IEnumerable<Command> GetCommands(int index, string[] parts)
		{
			if (index != parts.Length)
			{
				string nextPart = parts[index];
				CommandMap nextGroup;
				if (_items.TryGetValue(nextPart, out nextGroup))
				{
					var cmd = nextGroup.GetCommands(index + 1, parts);
					if (cmd != null)
						return cmd;
				}
			}

			if (_commands != null)
				return _commands;
			return null;
		}

		public void AddCommand(string text, Command command)
		{
			AddCommand(0, text.Split(' '), command);
		}
		private void AddCommand(int index, string[] parts, Command command)
		{
			if (!command.IsHidden && _isHidden)
				_isHidden = false;

			if (index != parts.Length)
			{
				CommandMap nextGroup;
				string name = parts[index];
				string fullName = string.Join(" ", parts, 0, index + 1);
                if (!_items.TryGetValue(name, out nextGroup))
				{
					nextGroup = new CommandMap(this, name, fullName);
					_items.Add(name, nextGroup);
				}
				nextGroup.AddCommand(index + 1, parts, command);
            }
			else
				_commands.Add(command);
		}

		public bool CanRun(User user, Channel channel)
		{
			if (_commands.Count > 0)
			{
				foreach (var cmd in _commands)
				{
					if (cmd.CanRun(user, channel))
						return true;
				}
			}
			if (_items.Count > 0)
			{
				foreach (var item in _items)
				{
					if (item.Value.CanRun(user, channel))
						return true;
				}
			}
			return false;
		}
	}
}
