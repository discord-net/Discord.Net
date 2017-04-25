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

        public Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, IServiceProvider services = null)
            => Command.CheckPreconditionsAsync(context, services);
        public Task<ParseResult> ParseAsync(ICommandContext context, SearchResult searchResult, PreconditionResult? preconditionResult = null)
            => Command.ParseAsync(context, Alias.Length, searchResult, preconditionResult);
        public Task<ExecuteResult> ExecuteAsync(ICommandContext context, IEnumerable<object> argList, IEnumerable<object> paramList, IServiceProvider services)
            => Command.ExecuteAsync(context, argList, paramList, services);
        public Task<ExecuteResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IServiceProvider services)
            => Command.ExecuteAsync(context, parseResult, services);
    }
}
