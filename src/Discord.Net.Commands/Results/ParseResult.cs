using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.Commands
{
    /// <summary>
    ///     Contains information for the parsing result from the command service's parser.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct ParseResult : IResult
    {
        public IReadOnlyList<TypeReaderResult> ArgValues { get; }
        public IReadOnlyList<TypeReaderResult> ParamValues { get; }

        /// <inheritdoc/>
        public CommandError? Error { get; }
        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <summary>
        ///     Provides information about the parameter that caused the parsing error.
        /// </summary>
        /// <returns>
        ///     A <see cref="ParameterInfo" /> indicating the parameter info of the error that may have occurred during parsing; 
        ///     <c>null</c> if the parsing was successful or the parsing error is not specific to a single parameter.
        /// </returns>
        public ParameterInfo ErrorParameter { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        private ParseResult(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues, CommandError? error, string errorReason, ParameterInfo errorParamInfo)
        {
            ArgValues = argValues;
            ParamValues = paramValues;
            Error = error;
            ErrorReason = errorReason;
            ErrorParameter = errorParamInfo;
        }

        public static ParseResult FromSuccess(IReadOnlyList<TypeReaderResult> argValues, IReadOnlyList<TypeReaderResult> paramValues)
        {
            for (int i = 0; i < argValues.Count; i++)
            {
                if (argValues[i].Values.Count > 1)
                    return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.", null);
            }
            for (int i = 0; i < paramValues.Count; i++)
            {
                if (paramValues[i].Values.Count > 1)
                    return new ParseResult(argValues, paramValues, CommandError.MultipleMatches, "Multiple matches found.", null);
            }
            return new ParseResult(argValues, paramValues, null, null, null);
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
            return new ParseResult(argList, paramList, null, null, null);
        }

        public static ParseResult FromError(CommandError error, string reason)
            => new ParseResult(null, null, error, reason, null);
        public static ParseResult FromError(CommandError error, string reason, ParameterInfo parameterInfo)
            => new ParseResult(null, null, error, reason, parameterInfo);
        public static ParseResult FromError(Exception ex)
            => FromError(CommandError.Exception, ex.Message);
        public static ParseResult FromError(IResult result)
            => new ParseResult(null, null, result.Error, result.ErrorReason, null);
        public static ParseResult FromError(IResult result, ParameterInfo parameterInfo)
            => new ParseResult(null, null, result.Error, result.ErrorReason, parameterInfo);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? $"Success ({ArgValues.Count}{(ParamValues.Count > 0 ? $" +{ParamValues.Count} Values" : "")})" : $"{Error}: {ErrorReason}";
    }
}
