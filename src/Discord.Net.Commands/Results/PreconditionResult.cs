using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class PreconditionResult : IResult
    {
        private static readonly PreconditionResult _success = new PreconditionResult(null, null);
        public static PreconditionResult Success => _success;

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private PreconditionResult(CommandError? error, string errorReason)
        {
            Error = error;
            ErrorReason = errorReason;
        }

        public static PreconditionResult FromError(string reason)
            => new PreconditionResult(CommandError.UnmetPrecondition, reason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
