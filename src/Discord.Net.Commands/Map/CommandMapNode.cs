using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Commands
{
    internal class CommandMapNode
    {
        private static readonly char[] _whitespaceChars = new char[] { ' ', '\r', '\n' };

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
            int nextSegment = NextCommandSegment(service, text, index);
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

                    var nextNode = _nodes.GetOrAdd(name, x => new CommandMapNode(x));
                    nextNode.AddCommand(service, nextSegment == -1 ? "" : text, nextSegment + 1, command);
                }
            }
        }
        public void RemoveCommand(CommandService service, string text, int index, CommandInfo command)
        {
            int nextSegment = NextCommandSegment(service, text, index);
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

        public IEnumerable<CommandInfo> GetCommands(CommandService service, string text, int index, bool lastLevel = false)
        {
            int nextCommand = NextCommandSegment(service, text, index);
            string name = null;

            //got all command segments
            if (nextCommand == -1)
            {
                //do we have parameters?
                int nextSpace = NextWhitespace(service, text, index);

                if (nextSpace != -1 && !lastLevel)
                    name = text.Substring(index, nextSpace - index);
                else
                    name = text.Substring(index);

                lastLevel = true;
                nextCommand = nextSpace;
            }
            else
            {
                name = text.Substring(index, nextCommand - index);
            }

            if (nextCommand == -1 || lastLevel)
            {
                var commands = _commands;
                for (int i = 0; i < commands.Length; i++)
                    yield return _commands[i];
            }

            if (name != null)
            {
                CommandMapNode nextNode;
                if (_nodes.TryGetValue(name, out nextNode))
                {
                    foreach (var cmd in nextNode.GetCommands(service, text, nextCommand + 1, lastLevel))
                        yield return cmd;
                }
            }
        }

        private static int NextCommandSegment(CommandService service, string text, int startIndex)
        {
            int lowest = int.MaxValue;

            int index = text.IndexOf(service._splitCharacter, startIndex);
            if (index != -1 && index < lowest)
                lowest = index;

            return (lowest != int.MaxValue) ? lowest : -1;
        }

        private static int NextWhitespace(CommandService service, string text, int startIndex)
        {
            int lowest = int.MaxValue;
            for (int i = 0; i < _whitespaceChars.Length; i++)
            {
                int index = text.IndexOf(_whitespaceChars[i], startIndex);
                if (index != -1 && index < lowest)
                    lowest = index;
            }

            return (lowest != int.MaxValue) ? lowest : -1;
        }
    }
}
