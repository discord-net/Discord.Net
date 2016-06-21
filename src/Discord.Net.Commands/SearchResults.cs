using System.Collections.Generic;

namespace Discord.Commands
{
    public struct SearchResults
    {
        IReadOnlyList<Command> Commands { get; }
        int ArgsPos { get; }

        public SearchResults(IReadOnlyList<Command> commands, int argsPos)
        {
            Commands = commands;
            ArgsPos = argsPos;
        }
    }
}
