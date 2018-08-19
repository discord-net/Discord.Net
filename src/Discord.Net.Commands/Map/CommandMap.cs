using System.Collections.Generic;

namespace Discord.Commands
{
    internal class CommandMap
    {
        private static readonly string[] _blankAliases = {""};
        private readonly CommandMapNode _root;
        private readonly CommandService _service;

        public CommandMap(CommandService service)
        {
            _service = service;
            _root = new CommandMapNode("");
        }

        public void AddCommand(CommandInfo command)
        {
            foreach (var text in command.Aliases)
                _root.AddCommand(_service, text, 0, command);
        }

        public void RemoveCommand(CommandInfo command)
        {
            foreach (var text in command.Aliases)
                _root.RemoveCommand(_service, text, 0, command);
        }

        public IEnumerable<CommandMatch> GetCommands(string text) => _root.GetCommands(_service, text, 0, text != "");
    }
}
