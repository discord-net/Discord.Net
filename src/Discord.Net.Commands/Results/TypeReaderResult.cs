using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct TypeReaderResult : IResult
    {
        public object Value { get; }

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private TypeReaderResult(object value, CommandError? error, string errorReason)
        {
            Value = value;
            Error = error;
            ErrorReason = errorReason;
        }

        public static TypeReaderResult FromSuccess(object value)
            => new TypeReaderResult(value, null, null);
        public static TypeReaderResult FromError(CommandError error, string reason)
            => new TypeReaderResult(null, error, reason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? $"Success ({Value})" : $"{Error}: {ErrorReason}";
    }
}
