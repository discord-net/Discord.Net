using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions
{
    internal class CommandMap<T> where T : class, ICommandInfo
    {
        private readonly char[] _seperators;

        private readonly CommandMapNode<T> _root;
        private readonly InteractionService _commandService;

        public IReadOnlyCollection<char> Seperators => _seperators;

        public CommandMap(InteractionService commandService, char[] seperators = null)
        {
            _seperators = seperators ?? Array.Empty<char>();

            _commandService = commandService;
            _root = new CommandMapNode<T>(null, _commandService._wildCardExp);
        }

        public void AddCommand(T command, bool ignoreGroupNames = false)
        {
            if (ignoreGroupNames)
                AddCommandToRoot(command);
            else
                AddCommand(command);
        }

        public void AddCommandToRoot(T command)
        {
            string[] key = new string[] { command.Name };
            _root.AddCommand(key, 0, command);
        }

        public void AddCommand(IList<string> input, T command)
        {
            _root.AddCommand(input, 0, command);
        }

        public void RemoveCommand(T command)
        {
            var key = CommandHierarchy.GetCommandPath(command);

            _root.RemoveCommand(key, 0);
        }

        public SearchResult<T> GetCommand(string input)
        {
            if (_seperators.Any())
                return GetCommand(input.Split(_seperators));
            else
                return GetCommand(new string[] { input });
        }

        public SearchResult<T> GetCommand(IList<string> input) =>
            _root.GetCommand(input, 0);

        private void AddCommand(T command)
        {
            var key = CommandHierarchy.GetCommandPath(command);

            _root.AddCommand(key, 0, command);
        }
    }
}
