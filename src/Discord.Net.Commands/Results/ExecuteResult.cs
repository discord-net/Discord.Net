using System;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct ExecuteResult : IResult
    {
        public Exception Exception { get; }

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private ExecuteResult(Exception exception, CommandError? error, string errorReason)
        {
            Exception = exception;
            Error = error;
            ErrorReason = errorReason;
        }

        internal static ExecuteResult FromSuccess()
            => new ExecuteResult(null, null, null);
        internal static ExecuteResult FromError(CommandError error, string reason)
            => new ExecuteResult(null, error, reason);
        internal static ExecuteResult FromError(Exception ex)
            => new ExecuteResult(ex, CommandError.Exception, ex.Message);
        internal static ExecuteResult FromError(ParseResult result)
            => new ExecuteResult(null, result.Error, result.ErrorReason);
        
        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
