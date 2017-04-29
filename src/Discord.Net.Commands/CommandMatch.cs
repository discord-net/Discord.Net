using System.Collections.Generic;
using System.Threading.Tasks;

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

        public Task<PreconditionResult> CheckPreconditionsAsync(ICommandContext context, IDependencyMap map = null)
            => Command.CheckPreconditionsAsync(context, map);
        public Task<ParseResult> ParseAsync(ICommandContext context, SearchResult searchResult)
            => Command.ParseAsync(context, searchResult.Text.Substring(Alias.Length));
        public Task<IResult> ExecuteAsync(ICommandContext context, IEnumerable<object> argList, IEnumerable<object> paramList, IDependencyMap map)
            => Command.ExecuteAsync(context, argList, paramList, map);
        public Task<IResult> ExecuteAsync(ICommandContext context, ParseResult parseResult, IDependencyMap map)
            => Command.ExecuteAsync(context, parseResult, map);
    }
}
