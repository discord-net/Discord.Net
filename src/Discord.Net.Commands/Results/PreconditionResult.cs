using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class PreconditionResult : IResult
    {
        /// <inheritdoc/>
        public CommandError? Error { get; }
        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        protected PreconditionResult(CommandError? error, string errorReason)
        {
            Error = error;
            ErrorReason = errorReason;
        }

        public static PreconditionResult FromSuccess()
            => new PreconditionResult(null, null);
        public static PreconditionResult FromError(string reason)
            => new PreconditionResult(CommandError.UnmetPrecondition, reason);
        public static PreconditionResult FromError(IResult result)
            => new PreconditionResult(result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
