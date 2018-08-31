using System;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct ExecuteResult : IResult
    {
        /// <summary>
        ///     Gets the exception that may have occurred during the command execution.
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc />
        public CommandError? Error { get; }
        /// <inheritdoc />
        public string ErrorReason { get; }

        /// <inheritdoc />
        public bool IsSuccess => !Error.HasValue;

        private ExecuteResult(Exception exception, CommandError? error, string errorReason)
        {
            Exception = exception;
            Error = error;
            ErrorReason = errorReason;
        }

        public static ExecuteResult FromSuccess()
            => new ExecuteResult(null, null, null);
        public static ExecuteResult FromError(CommandError error, string reason)
            => new ExecuteResult(null, error, reason);
        public static ExecuteResult FromError(Exception ex)
            => new ExecuteResult(ex, CommandError.Exception, ex.Message);
        public static ExecuteResult FromError(IResult result)
            => new ExecuteResult(null, result.Error, result.ErrorReason);
        
        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
