using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct ParseResult : IResult
    {
        public IReadOnlyList<object> Values { get; }

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private ParseResult(IReadOnlyList<object> values, CommandError? error, string errorReason)
        {
            Values = values;
            Error = error;
            ErrorReason = errorReason;
        }

        internal static ParseResult FromSuccess(IReadOnlyList<object> values)
            => new ParseResult(values, null, null);
        internal static ParseResult FromError(CommandError error, string reason)
            => new ParseResult(null, error, reason);
        internal static ParseResult FromError(SearchResult result)
            => new ParseResult(null, result.Error, result.ErrorReason);
        internal static ParseResult FromError(TypeReaderResult result)
            => new ParseResult(null, result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? $"Success ({Values.Count} Values)" : $"{Error}: {ErrorReason}";
    }
}
