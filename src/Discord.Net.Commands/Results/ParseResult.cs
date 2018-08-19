using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public struct ParseResult : IResult
    {
        public IReadOnlyList<TypeReaderResult> ArgValues { get; }
        public IReadOnlyList<TypeReaderResult> ParamValues { get; }

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private ParseResult(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues,
            CommandError? error, string errorReason)
        {
            ArgValues = argValues;
            ParamValues = paramValues;
            Error = error;
            ErrorReason = errorReason;
        }

        public static ParseResult FromSuccess(IReadOnlyList<TypeReaderResult> argValues,
            IReadOnlyList<TypeReaderResult> paramValues)
        {
            if (argValues.Any(t => t.Values.Count > 1))
                return new ParseResult(argValues, paramValues, CommandError.MultipleMatches,
                    "Multiple matches found.");
            if (paramValues.Any(t => t.Values.Count > 1))
                return new ParseResult(argValues, paramValues, CommandError.MultipleMatches,
                    "Multiple matches found.");

            return new ParseResult(argValues, paramValues, null, null);
        }

        public static ParseResult FromSuccess(IReadOnlyList<TypeReaderValue> argValues,
            IReadOnlyList<TypeReaderValue> paramValues)
        {
            var argList = new TypeReaderResult[argValues.Count];
            for (var i = 0; i < argValues.Count; i++)
                argList[i] = TypeReaderResult.FromSuccess(argValues[i]);
            TypeReaderResult[] paramList;
            if (paramValues == null) return new ParseResult(argList, null, null, null);
            {
                paramList = new TypeReaderResult[paramValues.Count];
                for (var i = 0; i < paramValues.Count; i++)
                    paramList[i] = TypeReaderResult.FromSuccess(paramValues[i]);
            }

            return new ParseResult(argList, paramList, null, null);
        }

        public static ParseResult FromError(CommandError error, string reason)
            => new ParseResult(null, null, error, reason);

        public static ParseResult FromError(Exception ex)
            => FromError(CommandError.Exception, ex.Message);

        public static ParseResult FromError(IResult result)
            => new ParseResult(null, null, result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

        private string DebuggerDisplay => IsSuccess
            ? $"Success ({ArgValues.Count}{(ParamValues.Count > 0 ? $" +{ParamValues.Count} Values" : "")})"
            : $"{Error}: {ErrorReason}";
    }
}
