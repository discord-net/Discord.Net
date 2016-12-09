using System.Collections.Generic;

namespace Discord.Commands
{
    internal class CommandMap
    {
        private readonly CommandMapNode _root;
        private static readonly string[] _blankAliases = new[] { "" };

        public CommandMap()
        {
            _root = new CommandMapNode("");
        }

        public void AddCommand(CommandInfo command, CommandService service)
        {
            foreach (string text in GetAliases(command))
                _root.AddCommand(service, text, 0, command);
        }
        public void RemoveCommand(CommandInfo command, CommandService service)
        {
            foreach (string text in GetAliases(command))
                _root.RemoveCommand(service, text, 0, command);
        }

        public IEnumerable<CommandInfo> GetCommands(string text, CommandService service)
        {
            return _root.GetCommands(service, text, 0);
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
