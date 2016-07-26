using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Commands
{
    internal class CommandMap
    {
        private readonly ConcurrentDictionary<string, CommandMapNode> _nodes;

        public CommandMap()
        {
            _nodes = new ConcurrentDictionary<string, CommandMapNode>();
        }

        public void AddCommand(Command command)
        {
            string text = command.Text;
            int nextSpace = text.IndexOf(' ');
            string name;

            lock (this)
            {
                if (nextSpace == -1)
                    name = command.Text;
                else
                    name = command.Text.Substring(0, nextSpace);
                var nextNode = _nodes.GetOrAdd(name, x => new CommandMapNode(x));
                nextNode.AddCommand(nextSpace == -1 ? "" : text, nextSpace + 1, command);
            }
        }
        public void RemoveCommand(Command command)
        {
            string text = command.Text;
            int nextSpace = text.IndexOf(' ');
            string name;

            lock (this)
            {
                if (nextSpace == -1)
                    name = command.Text;
                else
                    name = command.Text.Substring(0, nextSpace);

                CommandMapNode nextNode;
                if (_nodes.TryGetValue(name, out nextNode))
                {
                    nextNode.AddCommand(nextSpace == -1 ? "" : text, nextSpace + 1, command);
                    if (nextNode.IsEmpty)
                        _nodes.TryRemove(name, out nextNode);
                }
            }
        }

        public IEnumerable<Command> GetCommands(string text)
        {
            int nextSpace = text.IndexOf(' ');
            string name;

            lock (this)
            {
                if (nextSpace == -1)
                    name = text;
                else
                    name = text.Substring(0, nextSpace);

                CommandMapNode nextNode;
                if (_nodes.TryGetValue(name, out nextNode))
                    return nextNode.GetCommands(text, nextSpace + 1);
                else
                    return Enumerable.Empty<Command>();
            }
        }
    }
}
