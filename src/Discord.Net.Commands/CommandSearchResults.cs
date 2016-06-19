using System.Collections.Generic;

namespace Discord.Commands
{
    public struct CommandSearchResults
    {
        IReadOnlyList<Command> Commands { get; }
        int ArgsPos { get; }

        public CommandSearchResults(IReadOnlyList<Command> commands, int argsPos)
        {
            Commands = commands;
            ArgsPos = argsPos;
        }
    }
}
