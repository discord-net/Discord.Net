using System.Collections.Generic;

namespace Discord.Commands
{
    internal class CommandMap
    {
        private readonly CommandMapNode _root;
        private static readonly string[] _blankAliases = new[] { "" };

        public CommandMap(CommandService service)
        {
            _root = new CommandMapNode("", service);
        }

        public void AddCommand(CommandInfo command)
        {
            foreach (string text in GetAliases(command))
                _root.AddCommand(text, 0, command);
        }
        public void RemoveCommand(CommandInfo command)
        {
            foreach (string text in GetAliases(command))
                _root.RemoveCommand(text, 0, command);
        }

        public IEnumerable<CommandInfo> GetCommands(string text)
        {
            return _root.GetCommands(text, 0);
        }

        private IReadOnlyList<string> GetAliases(CommandInfo command)
        {
            var aliases = command.Aliases;
            if (aliases.Count == 0)
                return _blankAliases;
            return aliases;
        }
    }
}
