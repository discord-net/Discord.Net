using System;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class PreconditionResult : IResult
    {
        protected PreconditionResult(CommandError? error, string errorReason)
        {
            Error = error;
            ErrorReason = errorReason;
        }

        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        public static PreconditionResult FromSuccess()
            => new PreconditionResult(null, null);

        public static PreconditionResult FromError(string reason)
            => new PreconditionResult(CommandError.UnmetPrecondition, reason);

        public static PreconditionResult FromError(Exception ex)
            => new PreconditionResult(CommandError.Exception, ex.Message);

        public static PreconditionResult FromError(IResult result)
            => new PreconditionResult(result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
