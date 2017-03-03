using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Commands
{
    internal class CommandMapNode
    {
        private static readonly char[] _whitespaceChars = new[] { ' ', '\r', '\n' };

        private readonly ConcurrentDictionary<string, CommandMapNode> _nodes;
        private readonly string _name;
        private readonly object _lockObj = new object();
        private ImmutableArray<CommandInfo> _commands;

        public bool IsEmpty => _commands.Length == 0 && _nodes.Count == 0;

        public CommandMapNode(string name)
        {
            _name = name;
            _nodes = new ConcurrentDictionary<string, CommandMapNode>();
            _commands = ImmutableArray.Create<CommandInfo>();
        }

        public void AddCommand(CommandService service, string text, int index, CommandInfo command)
        {
            int nextSegment = NextSegment(text, index, service._separatorChar);
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
                    if (nextSegment == -1)
                        name = text.Substring(index);
                    else
                        name = text.Substring(index, nextSegment - index);

                    string fullName = _name == "" ? name : _name + service._separatorChar + name;
                    var nextNode = _nodes.GetOrAdd(name, x => new CommandMapNode(fullName));
                    nextNode.AddCommand(service, nextSegment == -1 ? "" : text, nextSegment + 1, command);
                }
            }
        }
        public void RemoveCommand(CommandService service, string text, int index, CommandInfo command)
        {
            int nextSegment = NextSegment(text, index, service._separatorChar);
            string name;

            lock (_lockObj)
            {
                if (text == "")
                    _commands = _commands.Remove(command);
                else
                {
                    if (nextSegment == -1)
                        name = text.Substring(index);
                    else
                        name = text.Substring(index, nextSegment - index);

                    CommandMapNode nextNode;
                    if (_nodes.TryGetValue(name, out nextNode))
                    {
                        nextNode.RemoveCommand(service, nextSegment == -1 ? "" : text, nextSegment + 1, command);
                        if (nextNode.IsEmpty)
                            _nodes.TryRemove(name, out nextNode);
                    }
                }
            }
        }

        public IEnumerable<CommandMatch> GetCommands(CommandService service, string text, int index, bool visitChildren = true)
        {
            var commands = _commands;
            for (int i = 0; i < commands.Length; i++)
                yield return new CommandMatch(_commands[i], _name);

            if (visitChildren)
            {
                string name;
                CommandMapNode nextNode;

                //Search for next segment
                int nextSegment = NextSegment(text, index, service._separatorChar);
                if (nextSegment == -1)
                    name = text.Substring(index);
                else
                    name = text.Substring(index, nextSegment - index);
                if (_nodes.TryGetValue(name, out nextNode))
                {
                    foreach (var cmd in nextNode.GetCommands(service, nextSegment == -1 ? "" : text, nextSegment + 1, true))
                        yield return cmd;
                }

                //Check if this is the last command segment before args
                nextSegment = NextSegment(text, index, _whitespaceChars, service._separatorChar);
                if (nextSegment != -1)
                {
                    name = text.Substring(index, nextSegment - index);
                    if (_nodes.TryGetValue(name, out nextNode))
                    {
                        foreach (var cmd in nextNode.GetCommands(service, nextSegment == -1 ? "" : text, nextSegment + 1, false))
                            yield return cmd;
                    }
                }
            }
        }

        private static int NextSegment(string text, int startIndex, char separator)
        {
            return text.IndexOf(separator, startIndex);
        }
        private static int NextSegment(string text, int startIndex, char[] separators, char except)
        {
            int lowest = int.MaxValue;
            for (int i = 0; i < separators.Length; i++)
            {
                if (separators[i] != except)
                {
                    int index = text.IndexOf(separators[i], startIndex);
                    if (index != -1 && index < lowest)
                        lowest = index;
                }
            }
            return (lowest != int.MaxValue) ? lowest : -1;
        }
    }
}
