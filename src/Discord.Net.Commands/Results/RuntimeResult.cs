using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RuntimeResult : IResult
    {
        protected RuntimeResult(CommandError? error, string reason)
        {
            Error = error;
            Reason = reason;
        }

        public CommandError? Error { get; }
        public string Reason { get; }

        public bool IsSuccess => !Error.HasValue;

        string IResult.ErrorReason => Reason;

        public static RuntimeResult FromSuccess(string reason = null) =>
            new RuntimeResult(null, reason);
        public static RuntimeResult FromError(string reason) =>
            new RuntimeResult(CommandError.Unsuccessful, reason);
        public static RuntimeResult FromError(IResult result) =>
            new RuntimeResult(result.Error, result.ErrorReason);

        public override string ToString() => Reason ?? (IsSuccess ? "Successful" : "Unsuccessful");
        private string DebuggerDisplay => IsSuccess ? $"Success: {Reason ?? "No Reason"}" : $"{Error}: {Reason}";
    }
}
