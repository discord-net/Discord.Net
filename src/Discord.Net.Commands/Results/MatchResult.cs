using System;

namespace Discord.Commands
{
    public class MatchResult : IResult
    {
        /// <summary>
        ///     Gets the command that may have matched during the command execution.
        /// </summary>
        public CommandMatch? Match { get; }

        /// <summary>
        ///     Gets on which pipeline stage the command may have matched or failed.
        /// </summary>
        public IResult Pipeline { get; }

        /// <inheritdoc />
        public CommandError? Error { get; }
        /// <inheritdoc />
        public string ErrorReason { get; }
        /// <inheritdoc />
        public bool IsSuccess => !Error.HasValue;

        private MatchResult(CommandMatch? match, IResult pipeline, CommandError? error, string errorReason)
        {
            Match = match;
            Error = error;
            Pipeline = pipeline;
            ErrorReason = errorReason;
        }

        public static MatchResult FromSuccess(CommandMatch match, IResult pipeline)
            => new MatchResult(match,pipeline,null, null);
        public static MatchResult FromError(CommandError error, string reason)
            => new MatchResult(null,null,error, reason);
        public static MatchResult FromError(Exception ex)
            => FromError(CommandError.Exception, ex.Message);
        public static MatchResult FromError(IResult result)
            => new MatchResult(null, null,result.Error, result.ErrorReason);
        public static MatchResult FromError(IResult pipeline, CommandError error, string reason)
            => new MatchResult(null, pipeline, error, reason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";

    }
}
