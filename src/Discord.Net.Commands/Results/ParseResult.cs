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

        public static ParseResult FromSuccess(IReadOnlyList<object> values)
            => new ParseResult(values, null, null);
        public static ParseResult FromError(CommandError error, string reason)
            => new ParseResult(null, error, reason);
        public static ParseResult FromError(IResult result)
            => new ParseResult(null, result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? $"Success ({Values.Count} Values)" : $"{Error}: {ErrorReason}";
    }
}
