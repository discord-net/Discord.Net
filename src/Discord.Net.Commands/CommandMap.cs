using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Commands
{
	//Represents either a single function, command group, or both
	internal class CommandMap
	{
		private readonly CommandMap _parent;
		private readonly string _text;

		private Command _command;
		private readonly Dictionary<string, CommandMap> _items;
		private int _minPermission;
		private bool _isHidden;

		public string Text => _text;
		public int MinPermissions => _minPermission;
		public bool IsHidden => _isHidden;
		public IEnumerable<Command> SubCommands => _items.Select(x => x.Value._command).Where(x => x != null);
		public IEnumerable<CommandMap> SubGroups => _items.Select(x => x.Value).Where(x => x._items.Count > 0);

		public CommandMap(CommandMap parent, string text)
		{
			_parent = parent;
			_text = text;
			_items = new Dictionary<string, CommandMap>();
			_minPermission = int.MaxValue;
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
				if (_items.TryGetValue(nextPart, out nextGroup))
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
				if (command.MinPermissions < _minPermission)
					_minPermission = command.MinPermissions;
				if (!command.IsHidden && _isHidden)
					_isHidden = false;

				string nextPart = parts[index];
				CommandMap nextGroup;
				if (!_items.TryGetValue(nextPart, out nextGroup))
				{
					nextGroup = new CommandMap(this, nextPart);
					_items.Add(nextPart, nextGroup);
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
