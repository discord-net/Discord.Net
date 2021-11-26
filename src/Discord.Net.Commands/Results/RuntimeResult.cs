using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class RuntimeResult : IResult
    {
        /// <summary>
        ///     Initializes a new <see cref="RuntimeResult" /> class with the type of error and reason.
        /// </summary>
        /// <param name="error">The type of failure, or <c>null</c> if none.</param>
        /// <param name="reason">The reason of failure.</param>
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
