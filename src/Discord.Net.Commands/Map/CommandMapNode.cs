using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Commands
{
    internal class CommandMapNode
    {
        private readonly ConcurrentDictionary<string, CommandMapNode> _nodes;
        private readonly string _name;
        private ImmutableArray<Command> _commands;

        public bool IsEmpty => _commands.Length == 0 && _nodes.Count == 0;

        public CommandMapNode(string name)
        {
            _name = name;
            _nodes = new ConcurrentDictionary<string, CommandMapNode>();
            _commands = ImmutableArray.Create<Command>();
        }

        public void AddCommand(string text, int index, Command command)
        {
            int nextSpace = text.IndexOf(' ', index);
            string name;

            lock (this)
            {
                if (text == "")
                    _commands = _commands.Add(command);
                else
                {
                    if (nextSpace == -1)
                        name = text.Substring(index);
                    else
                        name = text.Substring(index, nextSpace - index);

                    var nextNode = _nodes.GetOrAdd(name, x => new CommandMapNode(x));
                    nextNode.AddCommand(nextSpace == -1 ? "" : text, nextSpace + 1, command);
                }
            }
        }
        public void RemoveCommand(string text, int index, Command command)
        {
            int nextSpace = text.IndexOf(' ', index);
            string name;

            lock (this)
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

        public IEnumerable<Command> GetCommands(string text, int index)
        {
            int nextSpace = text.IndexOf(' ', index);
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
    }
}
