using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Commands
{
    public struct CommandMatch
    {
        public CommandInfo Command { get; }
        public string Alias { get; }

        public CommandMatch(CommandInfo command, string alias)
        {
            Command = command;
            Alias = alias;
        }
    }
}
