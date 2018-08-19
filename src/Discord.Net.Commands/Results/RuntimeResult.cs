using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public abstract class RuntimeResult : IResult
    {
        protected RuntimeResult(CommandError? error, string reason)
        {
            Error = error;
            Reason = reason;
        }

        public string Reason { get; }
        private string DebuggerDisplay => IsSuccess ? $"Success: {Reason ?? "No Reason"}" : $"{Error}: {Reason}";

        public CommandError? Error { get; }

        public bool IsSuccess => !Error.HasValue;

        string IResult.ErrorReason => Reason;

        public override string ToString() => Reason ?? (IsSuccess ? "Successful" : "Unsuccessful");
    }
}
