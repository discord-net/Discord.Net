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

        public CommandInfo Command { get; }

        public bool IsSuccess => !Error.HasValue;

        private ExecuteResult(CommandInfo command, Exception exception, CommandError? error, string errorReason)
        {
            Command = command;
            Exception = exception;
            Error = error;
            ErrorReason = errorReason;
        }

        public static ExecuteResult FromSuccess()
            => new ExecuteResult(null, null, null, null);
        public static ExecuteResult FromError(CommandError error, string reason)
            => new ExecuteResult(null, null, error, reason);
        public static ExecuteResult FromError(Exception ex)
            => new ExecuteResult(null, ex, CommandError.Exception, ex.Message);
        public static ExecuteResult FromError(IResult result)
            => new ExecuteResult(null, null, result.Error, result.ErrorReason);
        public static ExecuteResult FromSuccess(CommandInfo command)
            => new ExecuteResult(command, null, null, null);
        public static ExecuteResult FromError(CommandInfo command, CommandError error, string reason)
            => new ExecuteResult(command, null, error, reason);
        public static ExecuteResult FromError(CommandInfo command, Exception ex)
            => new ExecuteResult(command, ex, CommandError.Exception, ex.Message);
        public static ExecuteResult FromError(CommandInfo command, IResult result)
            => new ExecuteResult(command, null, result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
