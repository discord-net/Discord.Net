using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Discord.Commands
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct TypeReaderValue
    {
        public object Value { get; }
        public float Score { get; }

        public TypeReaderValue(object value, float score)
        {
            Value = value;
            Score = score;
        }

        public override string ToString() => Value?.ToString();
        private string DebuggerDisplay => $"[{Value}, {Math.Round(Score, 2)}]";
    }

    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct TypeReaderResult : IResult
    {
        public IReadOnlyCollection<TypeReaderValue> Values { get; }

        public CommandError? Error { get; }
        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private TypeReaderResult(IReadOnlyCollection<TypeReaderValue> values, CommandError? error, string errorReason)
        {
            Values = values;
            Error = error;
            ErrorReason = errorReason;
        }

        public static TypeReaderResult FromSuccess(object value)
            => new TypeReaderResult(ImmutableArray.Create(new TypeReaderValue(value, 1.0f)), null, null);
        public static TypeReaderResult FromSuccess(TypeReaderValue value)
            => new TypeReaderResult(ImmutableArray.Create(value), null, null);
        public static TypeReaderResult FromSuccess(IReadOnlyCollection<TypeReaderValue> values)
            => new TypeReaderResult(values, null, null);
        public static TypeReaderResult FromError(CommandError error, string reason)
            => new TypeReaderResult(null, error, reason);
        public static TypeReaderResult FromError(IResult result)
            => new TypeReaderResult(null, result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? $"Success ({string.Join(", ", Values)})" : $"{Error}: {ErrorReason}";
    }
}
