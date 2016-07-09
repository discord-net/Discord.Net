using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct SearchResult : IResult
    {
        public string Text { get; }
        public IReadOnlyList<Command> Commands { get; }

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private SearchResult(string text, IReadOnlyList<Command> commands, CommandError? error, string errorReason)
        {
            Text = text;
            Commands = commands;
            Error = error;
            ErrorReason = errorReason;
        }

        internal static SearchResult FromSuccess(string text, IReadOnlyList<Command> commands)
            => new SearchResult(text, commands, null, null);
        internal static SearchResult FromError(CommandError error, string reason)
            => new SearchResult(null, null, error, reason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? $"Success ({Commands.Count} Results)" : $"{Error}: {ErrorReason}";
    }
}
