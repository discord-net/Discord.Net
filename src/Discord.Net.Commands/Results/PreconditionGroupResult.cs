using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct PreconditionGroupResult : IResult
    {
        public CommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        public IReadOnlyCollection<PreconditionResult> Preconditions { get; }

        private PreconditionGroupResult(CommandError? error, string errorReason, ICollection<PreconditionResult> preconditions)
        {
            Error = error;
            ErrorReason = errorReason;
            Preconditions = (preconditions ?? Enumerable.Empty<PreconditionResult>().ToList()).ToReadOnlyCollection();
        }

        public static PreconditionGroupResult FromSuccess()
            => new PreconditionGroupResult(null, null, null);
        public static PreconditionGroupResult FromError(string reason, ICollection<PreconditionResult> preconditions)
            => new PreconditionGroupResult(CommandError.UnmetPrecondition, reason, preconditions);
        public static PreconditionGroupResult FromError(IResult result) //needed?
            => new PreconditionGroupResult(result.Error, result.ErrorReason, null);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
