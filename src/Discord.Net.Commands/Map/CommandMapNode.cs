using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Commands
{
    internal class CommandMapNode
    {
        private static readonly char[] _whitespaceChars = {' ', '\r', '\n'};
        private readonly object _lockObj = new object();
        private readonly string _name;

        private readonly ConcurrentDictionary<string, CommandMapNode> _nodes;
        private ImmutableArray<CommandInfo> _commands;

        public CommandMapNode(string name)
        {
            _name = name;
            _nodes = new ConcurrentDictionary<string, CommandMapNode>();
            _commands = ImmutableArray.Create<CommandInfo>();
        }

        public bool IsEmpty => _commands.Length == 0 && _nodes.Count == 0;

        public void AddCommand(CommandService service, string text, int index, CommandInfo command)
        {
            var nextSegment = NextSegment(text, index, service._separatorChar);

            lock (_lockObj)
                switch (text)
                {
                    case "" when _name == "":
                        throw new InvalidOperationException("Cannot add commands to the root node.");
                    case "":
                        _commands = _commands.Add(command);
                        break;
                    default:
                        var name = nextSegment == -1
                            ? text.Substring(index)
                            : text.Substring(index, nextSegment - index);

                        var fullName = _name == "" ? name : _name + service._separatorChar + name;
                        var nextNode = _nodes.GetOrAdd(name, x => new CommandMapNode(fullName));
                        nextNode.AddCommand(service, nextSegment == -1 ? "" : text, nextSegment + 1, command);
                        break;
                }
        }

        public void RemoveCommand(CommandService service, string text, int index, CommandInfo command)
        {
            var nextSegment = NextSegment(text, index, service._separatorChar);

            lock (_lockObj)
                if (text == "")
                    _commands = _commands.Remove(command);
                else
                {
                    var name = nextSegment == -1 ? text.Substring(index) : text.Substring(index, nextSegment - index);

                    if (!_nodes.TryGetValue(name, out var nextNode)) return;
                    nextNode.RemoveCommand(service, nextSegment == -1 ? "" : text, nextSegment + 1, command);
                    if (nextNode.IsEmpty)
                        _nodes.TryRemove(name, out nextNode);
                }
        }

        public IEnumerable<CommandMatch> GetCommands(CommandService service, string text, int index,
            bool visitChildren = true)
        {
            var commands = _commands;
            for (var i = 0; i < commands.Length; i++)
                yield return new CommandMatch(_commands[i], _name);

            if (!visitChildren) yield break;
            //Search for next segment
            var nextSegment = NextSegment(text, index, service._separatorChar);
            var name = nextSegment == -1 ? text.Substring(index) : text.Substring(index, nextSegment - index);
            if (_nodes.TryGetValue(name, out var nextNode))
                foreach (var cmd in nextNode.GetCommands(service, nextSegment == -1 ? "" : text, nextSegment + 1))
                    yield return cmd;

            //Check if this is the last command segment before args
            nextSegment = NextSegment(text, index, _whitespaceChars, service._separatorChar);
            if (nextSegment == -1) yield break;
            {
                name = text.Substring(index, nextSegment - index);
                if (!_nodes.TryGetValue(name, out nextNode)) yield break;
                foreach (var cmd in nextNode.GetCommands(service, nextSegment == -1 ? "" : text,
                    nextSegment + 1, false))
                    yield return cmd;
            }
        }

        private static int NextSegment(string text, int startIndex, char separator) =>
            text.IndexOf(separator, startIndex);

        private static int NextSegment(string text, int startIndex, char[] separators, char except)
        {
            var lowest = int.MaxValue;
            foreach (var t in separators)
                if (t != except)
                {
                    var index = text.IndexOf(t, startIndex);
                    if (index != -1 && index < lowest)
                        lowest = index;
                }

            return lowest != int.MaxValue ? lowest : -1;
        }
    }
}
