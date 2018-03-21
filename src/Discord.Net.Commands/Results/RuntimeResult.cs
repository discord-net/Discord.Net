using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class RuntimeResult : IResult
    {
        protected RuntimeResult(CommandError? error, string reason)
        {
            Error = error;
            Reason = reason;
        }

        /// <inheritdoc/>
        public CommandError? Error { get; }
        /// <summary> Describes the execution reason or result. </summary>
        public string Reason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        /// <inheritdoc/>
        string IResult.ErrorReason => Reason;

        public override string ToString() => Reason ?? (IsSuccess ? "Successful" : "Unsuccessful");
        private string DebuggerDisplay => IsSuccess ? $"Success: {Reason ?? "No Reason"}" : $"{Error}: {Reason}";
    }
}
