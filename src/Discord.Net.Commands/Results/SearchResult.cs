using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SearchResult : IResult
    {
        public IReadOnlyList<Command> Commands { get; }
        public string ArgText { get; }        

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private SearchResult(IReadOnlyList<Command> commands, string argText, CommandError? error, string errorReason)
        {
            Commands = commands;
            ArgText = argText;
            Error = error;
            ErrorReason = errorReason;
        }

        internal static SearchResult FromSuccess(IReadOnlyList<Command> commands, string argText)
            => new SearchResult(commands, argText, null, null);
        internal static SearchResult FromError(CommandError error, string reason)
            => new SearchResult(null, null, error, reason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? $"Success ({Commands.Count} Results)" : $"{Error}: {ErrorReason}";
    }
}
