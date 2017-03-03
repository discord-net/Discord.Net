using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct ParseResult : IResult
    {
        public IReadOnlyList<TypeReaderResult> ArgValues { get; }
        public IReadOnlyList<TypeReaderResult> ParamValues { get; }

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private ParseResult(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues, CommandError? error, string errorReason)
        {
            ArgValues = argValues;
            ParamValues = paramValues;
            Error = error;
            ErrorReason = errorReason;
        }

        public static ParseResult FromSuccess(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues)
        {
            for (int i = 0; i < argValues.Count; i++)
            {
                if (argValues[i].Values.Count > 1)
                    return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.");
            }
            for (int i = 0; i < paramValues.Count; i++)
            {
                if (paramValues[i].Values.Count > 1)
                    return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.");
            }
            return new ParseResult(argValues, paramValues, null, null);
        }
        public static ParseResult FromSuccess(IReadOnlyList<TypeReaderValue> argValues, IReadOnlyList<TypeReaderValue> paramValues)
        {
            var argList = new TypeReaderResult[argValues.Count];
            for (int i = 0; i < argValues.Count; i++)
                argList[i] = TypeReaderResult.FromSuccess(argValues[i]);
            TypeReaderResult[] paramList = null;
            if (paramValues != null)
            {
                paramList = new TypeReaderResult[paramValues.Count];
                for (int i = 0; i < paramValues.Count; i++)
                    paramList[i] = TypeReaderResult.FromSuccess(paramValues[i]);
            }
            return new ParseResult(argList, paramList, null, null);
        }

        public static ParseResult FromError(CommandError error, string reason)
            => new ParseResult(null, null, error, reason);
        public static ParseResult FromError(IResult result)
            => new ParseResult(null, null, result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? $"Success ({ArgValues.Count}{(ParamValues.Count > 0 ? $" +{ParamValues.Count} Values" : "")})" : $"{Error}: {ErrorReason}";
    }
}
