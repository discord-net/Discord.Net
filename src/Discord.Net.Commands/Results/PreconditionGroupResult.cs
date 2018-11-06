using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class PreconditionGroupResult : PreconditionResult
    {
        public IReadOnlyCollection<PreconditionResult> PreconditionResults { get; }

        protected PreconditionGroupResult(CommandError? error, string errorReason, ICollection<PreconditionResult> preconditions)
            : base(error, errorReason)
        {
            PreconditionResults = (preconditions ?? new List<PreconditionResult>(0)).ToReadOnlyCollection();
        }

        public new static PreconditionGroupResult FromSuccess()
            => new PreconditionGroupResult(null, null, null);
        public static PreconditionGroupResult FromError(string reason, ICollection<PreconditionResult> preconditions)
            => new PreconditionGroupResult(CommandError.UnmetPrecondition, reason, preconditions);
        public static new PreconditionGroupResult FromError(Exception ex)
            => new PreconditionGroupResult(CommandError.Exception, ex.Message, null);
        public static new PreconditionGroupResult FromError(IResult result) //needed?
            => new PreconditionGroupResult(result.Error, result.ErrorReason, null);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
