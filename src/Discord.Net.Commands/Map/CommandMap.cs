using System.Collections.Generic;

namespace Discord.Commands
{
    internal class CommandMap
    {
        private readonly CommandService _service;
        private readonly CommandMapNode _root;
        private static readonly string[] _blankAliases = new[] { "" };

        public CommandMap(CommandService service)
        {
            _service = service;
            _root = new CommandMapNode("");
        }

        public void AddCommand(CommandInfo command)
        {
            foreach (string text in command.Aliases)
                _root.AddCommand(_service, text, 0, command);
        }
        public void RemoveCommand(CommandInfo command)
        {
            foreach (string text in command.Aliases)
                _root.RemoveCommand(_service, text, 0, command);
        }

        public IEnumerable<CommandMatch> GetCommands(string text)
        {
            return _root.GetCommands(_service, text, 0, text != "");
        }
    }
}
