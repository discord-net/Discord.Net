using System;

namespace Discord.Interactions
{
    internal struct ParseResult : IResult
    {
        public object Value { get; }

        public InteractionCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private ParseResult(object value, InteractionCommandError? error, string reason)
        {
            Value = value;
            Error = error;
            ErrorReason = reason;
        }

        public static ParseResult FromSuccess(object value) =>
            new ParseResult(value, null, null);

        public static ParseResult FromError(Exception exception) =>
            new ParseResult(null, InteractionCommandError.Exception, exception.Message);

        public static ParseResult FromError(InteractionCommandError error, string reason) =>
            new ParseResult(null, error, reason);

        public static ParseResult FromError(IResult result) =>
            new ParseResult(null, result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
