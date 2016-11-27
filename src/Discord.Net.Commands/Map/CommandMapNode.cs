using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Commands
{
    internal class CommandMapNode
    {
        private readonly ConcurrentDictionary<string, CommandMapNode> _nodes;
        private readonly string _name;
        private readonly object _lockObj = new object();
        private readonly CommandService _service;
        private ImmutableArray<CommandInfo> _commands;

        public bool IsEmpty => _commands.Length == 0 && _nodes.Count == 0;

        public CommandMapNode(string name, CommandService service)
        {
            _name = name;
            _nodes = new ConcurrentDictionary<string, CommandMapNode>();
            _commands = ImmutableArray.Create<CommandInfo>();
            _service = service;
        }

        public void AddCommand(string text, int index, CommandInfo command)
        {
            int nextSpace = NextSeparator(text, index);
            string name;

            lock (_lockObj)
            {
                if (text == "")
                {
                    if (_name == "")
                        throw new InvalidOperationException("Cannot add commands to the root node.");
                    _commands = _commands.Add(command);
                }
                else
                {
                    if (nextSpace == -1)
                        name = text.Substring(index);
                    else
                        name = text.Substring(index, nextSpace - index);

                    var nextNode = _nodes.GetOrAdd(name, x => new CommandMapNode(x, _service));
                    nextNode.AddCommand(nextSpace == -1 ? "" : text, nextSpace + 1, command);
                }
            }
        }
        public void RemoveCommand(string text, int index, CommandInfo command)
        {
            int nextSpace = NextSeparator(text, index);
            string name;

            lock (_lockObj)
            {
                if (text == "")
                    _commands = _commands.Remove(command);
                else
                {
                    if (nextSpace == -1)
                        name = text.Substring(index);
                    else
                        name = text.Substring(index, nextSpace - index);

                    CommandMapNode nextNode;
                    if (_nodes.TryGetValue(name, out nextNode))
                    {
                        nextNode.RemoveCommand(nextSpace == -1 ? "" : text, nextSpace + 1, command);
                        if (nextNode.IsEmpty)
                            _nodes.TryRemove(name, out nextNode);
                    }
                }
            }
        }

        public IEnumerable<CommandInfo> GetCommands(string text, int index)
        {
            int nextSpace = NextSeparator(text, index);
            string name;

            var commands = _commands;
            for (int i = 0; i < commands.Length; i++)
                yield return _commands[i];

            if (text != "")
            {
                if (nextSpace == -1)
                    name = text.Substring(index);
                else
                    name = text.Substring(index, nextSpace - index);

                CommandMapNode nextNode;
                if (_nodes.TryGetValue(name, out nextNode))
                {
                    foreach (var cmd in nextNode.GetCommands(nextSpace == -1 ? "" : text, nextSpace + 1))
                        yield return cmd;
                }
            }
        }

        private int NextSeparator(string text, int startIndex)
        {
            int lowest = int.MaxValue;
            int index = text.IndexOf(_service._nodeSeparator, startIndex);
            if (index != -1 && index < lowest)
                    lowest = index;
            return (lowest != int.MaxValue) ? lowest : -1;
        }
    }
}
