using System;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class PreconditionResult : IResult
    {
        public Exception Exception { get; }

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        protected PreconditionResult(CommandError? error, string errorReason, Exception exception = null)
        {
            Exception = exception;
            Error = error;
            ErrorReason = errorReason;
        }

        public static PreconditionResult FromSuccess()
            => new PreconditionResult(null, null);
        public static PreconditionResult FromError(string reason)
            => new PreconditionResult(CommandError.UnmetPrecondition, reason);
        public static PreconditionResult FromError(Exception ex)
            => new PreconditionResult(CommandError.Exception, ex.Message, ex);
        public static PreconditionResult FromError(IResult result)
            => new PreconditionResult(result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
